namespace PrimeMarket.Contracts.Products;

public record ProductCustomerResponse(
    int Id,
    string Name,
    decimal Price,
    bool InStock,
    string? PrimaryImageUrl,
    string? CategoryName,
    double AverageRating,
    int ReviewCount
);
