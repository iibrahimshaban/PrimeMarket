namespace PrimeMarket.Contracts.WishList;

public record WishlistItemResponse(
    int Id,              
    int ProductId,
    string Name,
    decimal Price,
    bool IsAvailable,  
    string? ImageUrl,  
    string? Category,
    double Rating,
    int ReviewCount
);
