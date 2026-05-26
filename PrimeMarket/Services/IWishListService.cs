using PrimeMarket.Contracts.WishList;

namespace PrimeMarket.Services;

public interface IWishListService
{
    Task<IEnumerable<WishlistItemResponse>> GetUserWishlistAsync(string userId, CancellationToken ct);
    Task<Result> AddToWishListAsync(int productId, string userId, CancellationToken ct);
    Task<Result> RemoveFromWishListAsync(int productId, string userId,CancellationToken ct);

}
