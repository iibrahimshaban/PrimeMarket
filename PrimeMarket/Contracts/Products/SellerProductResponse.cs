namespace PrimeMarket.Contracts.Products
{
    public record SellerProductResponse(
     int Id,
     string Name,
     decimal Price,
     int Stock,
     bool IsActive,
     string? PrimaryImageUrl,
     double AverageRating,
     int ReviewCount,
     string? CategoryName
    );
}
