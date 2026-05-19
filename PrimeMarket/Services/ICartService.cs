using PrimeMarket.Contracts.Cart;

namespace PrimeMarket.Services;

public interface ICartService
{
    Task<Result<CartResponse>> GetCartAsync(string userId, CancellationToken ct);
    Task<Result> AddToCartAsync(int productId, string userId, int quantity, CancellationToken ct);
    Task<Result> UpdateQuantityAsync(int cartItemId, string userId, int quantity, CancellationToken ct);
    Task<Result> RemoveFromCartAsync(int cartItemId, string userId, CancellationToken ct);
}
