namespace PrimeMarket.Contracts.Cart;

public record CartItemResponse(
    int Id,
    int ProductId,
    string Name,
    decimal Price,
    int Quantity,
    decimal Subtotal,      
    string? ImageUrl,
    bool IsAvailable,
    int Stock             
);
