namespace PrimeMarket.Contracts.Orders;

public record OrderItemResponse(
    int Id,
    string Name,
    string ImageUrl,
    int Quantity,
    decimal Subtotal
);
