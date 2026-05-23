using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;
using PrimeMarket.Errors;
using Stripe;

namespace PrimeMarket.Services;

public class OrderService(ApplicationDbContext contextt) : IOrderService
{
    private readonly ApplicationDbContext _context = contextt;

    public async Task<Result<PromoCodeValidationResponse>> ValidatePromoCodeAsync(
        string code, decimal cartTotal)
    {
        var promo = await _context.PromoCodes
            .FirstOrDefaultAsync(p =>
                p.Code == code &&
                p.IsActive &&
                p.ExpiresAt > DateTime.UtcNow &&
                p.UsedCount < p.UsageLimit);

        if (promo is null)
            return Result.Success(new PromoCodeValidationResponse(false, 0, "Invalid or expired promo code."));

        var discount = promo.DiscountType == DiscountType.Percentage
            ? cartTotal * (promo.DiscountValue / 100)
            : promo.DiscountValue;

        discount = Math.Min(discount, cartTotal); // never exceed total

        return Result.Success(new PromoCodeValidationResponse(true, discount, null));
    }

    public async Task<Result<PlaceOrderResponse>> PlaceOrderAsync(string userId, PlaceOrderRequest request)
    {
        // 1. load cart
        var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == userId && ci.Product.IsActive)
            .ToListAsync();

        if (!cartItems.Any())
            return Result.Failure<PlaceOrderResponse>(OrderError.CartEmpty);

        // 2. validate address belongs to user
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId);

        if (address is null)
            return Result.Failure<PlaceOrderResponse>(OrderError.AddressNotFound);

        // 3. apply promo code
        decimal discountAmount = 0;
        PromoCode? promo = null;
        decimal subtotal = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            promo = await _context.PromoCodes
                .FirstOrDefaultAsync(p =>
                    p.Code == request.PromoCode &&
                    p.IsActive &&
                    p.ExpiresAt > DateTime.UtcNow &&
                    p.UsedCount < p.UsageLimit);

            if (promo is null)
                return Result.Failure<PlaceOrderResponse>(OrderError.InvalidPromoCode);

            discountAmount = promo.DiscountType == DiscountType.Percentage
                ? subtotal * (promo.DiscountValue / 100)
                : promo.DiscountValue;

            discountAmount = Math.Min(discountAmount, subtotal);
            promo.UsedCount++;
        }

        decimal totalAmount = subtotal - discountAmount;

        // 4. create order
        var order = new Order
        {
            UserId = userId,
            AddressId = request.AddressId,
            PromoCodeId = promo?.Id,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            PaymentMethod = request.PaymentMethod,
            Status = OrderStatus.Pending,
            Items = cartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.Price
            }).ToList()
        };

        // 5. handle Stripe payment intent
        string? clientSecret = null;

        if (request.PaymentMethod == PaymentType.CreditCard)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(totalAmount * 100),
                    Currency = "egp",
                    Metadata = new Dictionary<string, string> { { "UserId", userId } }
                };
                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);
                clientSecret = intent.ClientSecret;
                order.PaymentRef = intent.Id;
            }
            catch (StripeException ex)
            {
                return Result.Failure<PlaceOrderResponse>(
                    new Error("Payment.Failed", ex.Message,StatusCodes.Status400BadRequest));
            }
        }

        // 6. clear cart
        _context.CartItems.RemoveRange(cartItems);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Result.Success(new PlaceOrderResponse(
            order.Id,
            totalAmount,
            discountAmount,
            clientSecret
        ));
    }

    public async Task<Result<GetOrderResponse>> GetOrderByIdAsync(string userId, string id)
    {
        if (!int.TryParse(id, out var orderId))
            return Result.Failure<GetOrderResponse>(OrderError.NotFound);

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null)
            return Result.Failure<GetOrderResponse>(OrderError.NotFound);

        return Result.Success(new GetOrderResponse(
            order.Id,
            order.TotalAmount,
            order.DiscountAmount,
            $"{order.Address.Street}, {order.Address.City}, {order.Address.Country}",
            order.CreatedOn.AddDays(7),
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.Product.Name,
                i.Product.Images.FirstOrDefault()?.Url ?? "",
                i.Quantity,
                i.UnitPrice * i.Quantity
            )).ToList()
        ));
    }
}
