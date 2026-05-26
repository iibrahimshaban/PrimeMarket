namespace PrimeMarket.Contracts.Brand;

public record BrandResponse(
    int Id,
    string BrandName,
    string? Description,
    string? LogoUrl,
    bool IsActive,
    bool IsVerified,
    string City,
    string Country,
    double AverageRating,
    int TotalProducts
);
