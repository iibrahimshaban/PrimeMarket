using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;
using PrimeMarket.Errors;

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

        var discount = promo.DiscountType == DiscountType.Percent
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

            discountAmount = promo.DiscountType == DiscountType.Percent
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

        //if (request.PaymentMethod == PaymentType.Card)
        //{
        //    StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

        //    var options = new PaymentIntentCreateOptions
        //    {
        //        Amount = (long)(totalAmount * 100),
        //        Currency = "usd",
        //        Metadata = new Dictionary<string, string>
        //        {
        //            { "UserId", userId }
        //        }
        //    };

        //    var service = new PaymentIntentService();
        //    var intent = await service.CreateAsync(options);

        //    clientSecret = intent.ClientSecret;
        //    order.PaymentRef = intent.Id;
        //}

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
}
