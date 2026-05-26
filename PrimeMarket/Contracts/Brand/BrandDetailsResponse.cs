namespace PrimeMarket.Contracts.Brand;

public record BrandDetailsResponse(
    int Id,
    string BrandName,
    string? Description,
    string? LogoUrl,
    bool IsActive,
    bool IsVerified,
    string Street,
    string City,
    string Country,
    double? Latitude,
    double? Longitude,
    string SellerName,
    double AverageRating,
    int TotalReviews,
    ICollection<BrandProductResponse> Products
);
