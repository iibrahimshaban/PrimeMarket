namespace PrimeMarket.Contracts.Products;

public record ProductDetailCustomerResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    string SellerName,
    string? PrimaryImageUrl,
    List<string> ImageUrls,
    List<string> Categories,
    double AverageRating,
    int ReviewCount,
    List<ProductReviewResponse> Reviews
);
