namespace PrimeMarket.Contracts.Products;
public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    string BrandName,
    int Stock,
    List<int> CategoryIds,
    IFormFile PrimaryImage,
    List<IFormFile>? ExtraImages
);