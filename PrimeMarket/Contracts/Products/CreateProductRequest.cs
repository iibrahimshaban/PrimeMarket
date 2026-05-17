namespace PrimeMarket.Contracts.Products;
public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    List<int> CategoryIds,
    IFormFile PrimaryImage,
    List<IFormFile>? ExtraImages
);