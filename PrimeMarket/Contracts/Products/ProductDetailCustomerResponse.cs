namespace PrimeMarket.Contracts.Products;

public record ProductDetailCustomerResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    string? BrandName,
    string? ShopName,
    int ShopId,
    string PrimaryImageUrl,
    ICollection<string> ImageUrls,
    ICollection<string> Categories,
    double AverageRating,
    int ReviewCount,
    ICollection<ProductReviewResponse> Reviews
);
