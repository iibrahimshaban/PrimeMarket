namespace PrimeMarket.Contracts.Orders;

public record PlaceOrderResponse(
    int OrderId,
    decimal TotalAmount,
    decimal DiscountAmount,
    string? ClientSecret  // null if Cash on Delivery
);
