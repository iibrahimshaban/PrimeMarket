namespace PrimeMarket.Contracts.Products
{
    public record SellerProductResponse(
     int Id,
     string Name,
     decimal Price,
     string? BrandName,
     int Stock,
     bool IsActive,
     string? PrimaryImageUrl,
     double AverageRating,
     int ReviewCount,
     IReadOnlyList<string> Categories,
     DateTime CreatedAt
    );
}
