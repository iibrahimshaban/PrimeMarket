namespace PrimeMarket.Contracts.Products;

public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    bool InStock,
    string PrimaryImageUrl,
    string SellerName,
    IReadOnlyList<string> Categories,
    double AverageRating,
    int ReviewCount,
    int OrderCount
);
