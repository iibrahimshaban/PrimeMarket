namespace PrimeMarket.Contracts.Products;

public record ProductReviewResponse(
    string UserName,
    string? UserAvatar,
    int Rating,
    string? Comment,
    DateTime CreatedAt
 );
