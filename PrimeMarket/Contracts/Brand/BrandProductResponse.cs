namespace PrimeMarket.Contracts.Brand;

public record BrandProductResponse(
    int Id,
    string Name,
    string? BrandName,
    decimal Price,
    int Stock,
    bool IsActive,
    string? PrimaryImageUrl,
    double AverageRating,
    int ReviewCount
);
