namespace PrimeMarket.Contracts.Products;

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    List<int> CategoryIds
);