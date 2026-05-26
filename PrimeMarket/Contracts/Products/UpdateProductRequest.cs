namespace PrimeMarket.Contracts.Products;

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    string? BrandName,
    List<int> CategoryIds
);