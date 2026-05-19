namespace PrimeMarket.Contracts.Cart;

public record CartResponse(
    IEnumerable<CartItemResponse> Items,
    decimal Total,         
    int ItemCount       
);
