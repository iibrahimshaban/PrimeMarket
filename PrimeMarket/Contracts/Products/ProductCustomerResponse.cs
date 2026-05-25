namespace PrimeMarket.Contracts.Products;

public record ProductCustomerResponse(
    int Id,
    string Name,
    decimal Price,
    bool InStock,
    string? PrimaryImageUrl,
    List<string> Categories,
    double AverageRating,
    int ReviewCount
);
