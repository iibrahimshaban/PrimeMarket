using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;
using PrimeMarket.Helpers;
using Stripe;

namespace PrimeMarket.Services;

public class OrderService(ApplicationDbContext contextt, INotificationService notificationService) : IOrderService
{
    private readonly ApplicationDbContext _context = contextt;
    private readonly INotificationService _notificationService = notificationService;

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

        var insufficientStock = cartItems
            .Where(ci => ci.Product.Stock < ci.Quantity)
            .Select(ci => ci.Product.Name)
            .ToList();

        if (insufficientStock.Count != 0)
            return Result.Failure<PlaceOrderResponse>(ProductError.InsufficientStock);

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
        foreach (var item in cartItems)
            item.Product.Stock -= item.Quantity;

        _context.CartItems.RemoveRange(cartItems);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _notificationService.SendToUserAsync(
            userId,
            "Order Placed",
            $"Your order #{order.Id} has been placed successfully. Total: {totalAmount:C}",
            "order"
        );

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
    // -----------------------------------------------------------------------

    public async Task<PaginationList<SellerOrderResponse>> GetSellerOrdersAsync(
        string sellerId,
        RequestFilter filter,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchValue))
        {
            var search = filter.SearchValue.Trim().ToLower();

            query = query.Where(o =>
                o.User.FirstName.ToLower().Contains(search) ||
                o.User.LastName.ToLower().Contains(search) ||
                o.Id.ToString().Contains(search));
        }

        var isDesc = filter.SortDirection.Equals(
            "DESC",
            StringComparison.OrdinalIgnoreCase);

        query = filter.SortColumn?.ToLower() switch
        {
            "amount" => isDesc
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),

            "status" => isDesc
                ? query.OrderByDescending(o => o.Status)
                : query.OrderBy(o => o.Status),

            "customer" => isDesc
                ? query.OrderByDescending(o => o.User.FirstName)
                : query.OrderBy(o => o.User.FirstName),

            _ => isDesc
                ? query.OrderByDescending(o => o.CreatedOn)
                : query.OrderBy(o => o.CreatedOn)
        };

        var projected = OrderExtension.MapSellerOrders(query, sellerId);

        return await PaginationList<SellerOrderResponse>.CreateAsync(
            projected,
            filter.PageNumber,
            filter.PageSize
        );
    }

    // -----------------------------------------------------------------------
    public async Task<PaginationList<CustomerOrderResponse>> GetCustomerOrdersAsync(
       string customerId,
       RequestFilter filter,
       CancellationToken cancellationToken)
    {
        var query = _context.Orders.Where(o => o.UserId == customerId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchValue))
        {
            var search = filter.SearchValue.Trim().ToLower();

            query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.Items.Any(i =>
                        i.Product.Seller.FirstName.ToLower().Contains(search) ||
                        i.Product.Seller.LastName.ToLower().Contains(search)));
        }

        var isDesc = filter.SortDirection.Equals(
            "DESC",
            StringComparison.OrdinalIgnoreCase);

        query = filter.SortColumn?.ToLower() switch
        {
            "amount" => isDesc
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),

            "status" => isDesc
                ? query.OrderByDescending(o => o.Status)
                : query.OrderBy(o => o.Status),

            "createdon" => isDesc
                ? query.OrderByDescending(p => p.CreatedOn)
                : query.OrderBy(p => p.CreatedOn),

            _ => query.OrderByDescending(p => p.CreatedOn)
        };

        var projected = query.ProjectToType<CustomerOrderResponse>();

        return await PaginationList<CustomerOrderResponse>.CreateAsync(
            projected,
            filter.PageNumber,
            filter.PageSize
        );
    }

    // -----------------------------------------------------------------------

    public async Task<PaginationList<AdminOrderResponse>> GetAdminOrdersAsync(
        RequestFilter filter,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchValue))
        {
            var search = filter.SearchValue.Trim().ToLower();

            query = query.Where(o =>
                o.User.FirstName.ToLower().Contains(search) ||
                o.User.LastName.ToLower().Contains(search) ||
                o.Id.ToString().Contains(search));
        }

        var isDesc = filter.SortDirection.Equals(
            "DESC",
            StringComparison.OrdinalIgnoreCase);

        query = filter.SortColumn?.ToLower() switch
        {
            "amount" => isDesc
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),

            "status" => isDesc
                ? query.OrderByDescending(o => o.Status)
                : query.OrderBy(o => o.Status),

            "customer" => isDesc
                ? query.OrderByDescending(o => o.User.FirstName)
                : query.OrderBy(o => o.User.FirstName),

            _ => isDesc
                ? query.OrderByDescending(o => o.CreatedOn)
                : query.OrderBy(o => o.CreatedOn)
        };

        var projected = query.ProjectToType<AdminOrderResponse>();

        return await PaginationList<AdminOrderResponse>.CreateAsync(
            projected,
            filter.PageNumber,
            filter.PageSize
        );
    }

    // -----------------------------------------------------------------------
    public async Task<Result<SellerOrderResponse>> GetSellerOrderByIdAsync(
        string sellerId,
        int orderId)
    {
        var query = _context.Orders
            .Where(o =>
                o.Id == orderId &&
                o.Items.Any(i => i.Product.SellerId == sellerId));

        var order = await OrderExtension.MapSellerOrders(query, sellerId)
            .FirstOrDefaultAsync();

        if (order is null)
            return Result.Failure<SellerOrderResponse>(
                OrderError.OrderNotFound);

        return Result.Success(order);
    }

    // -----------------------------------------------------------------------
    public async Task<Result> UpdateOrderStatusAsync(string userId, int orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
            return Result.Failure(OrderError.OrderNotFound);

        var isSellerOwner = order.Items.Any(i => i.Product.SellerId == userId);
        var isCustomerOwner = order.UserId == userId;

        if (!isSellerOwner && isCustomerOwner)
        {
            if (!(order.Status == OrderStatus.Pending && newStatus == OrderStatus.Cancelled))
                return Result.Failure(OrderError.UnauthorizedAction);
        }

        if (newStatus == OrderStatus.Cancelled)
        {
            foreach (var item in order.Items)
            {
                item.Product.Stock += item.Quantity;
            }
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync();

        var (title, message) = order.Status switch
        {
            OrderStatus.Confirmed => ("Order Confirmed", $"Your order #{order.Id} has been confirmed and is being prepared."),
            OrderStatus.Shipped => ("Order Shipped", $"Your order #{order.Id} is on its way!"),
            OrderStatus.Delivered => ("Order Delivered", $"Your order #{order.Id} has been delivered. Enjoy!"),
            OrderStatus.Cancelled => ("Order Cancelled", $"Your order #{order.Id} has been cancelled."),
            _ => ("Order Updated", $"Your order #{order.Id} status has been updated to {order.Status}.")
        };

        await _notificationService.SendToUserAsync(order.UserId, title, message, "order");

        return Result.Success();
    }
}
