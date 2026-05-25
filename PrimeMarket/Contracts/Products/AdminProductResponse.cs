namespace PrimeMarket.Contracts.Products
{
    public record AdminProductResponse(
     int Id,
     string Name,
     decimal Price,
     int Stock,
     bool IsActive,
     string? PrimaryImageUrl,
     double AverageRating,
     int ReviewCount,
     IReadOnlyList<string> Categories,
     string SellerName,
     DateTime CreatedAt
    );
}
