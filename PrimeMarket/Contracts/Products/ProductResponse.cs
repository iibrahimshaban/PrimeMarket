namespace PrimeMarket.Contracts.Products;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    bool InStock,
    string Thumbnail,
    IReadOnlyList<ProductImageResponse> Images,
    string SellerName,
    IReadOnlyList<string> Categories,
    double AverageRating,
    int ReviewCount,
    int OrderCount
);
