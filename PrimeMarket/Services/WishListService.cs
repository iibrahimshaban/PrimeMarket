using PrimeMarket.Contracts.WishList;
using PrimeMarket.Errors;

namespace PrimeMarket.Services;

public class WishListService(ApplicationDbContext context) : IWishListService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddToWishListAsync(int productId, string userId, CancellationToken ct)
    {
        var productExists = await _context.Products
        .AnyAsync(p => p.Id == productId && p.IsActive, ct);

        if (!productExists)
            return Result.Failure(WishListErrors.ProductNotFound);

        var alreadyExists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId, ct);

        if (alreadyExists)
            return Result.Failure(WishListErrors.AlreadyInWishlist);

        var wishlistItem = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        await _context.Wishlists.AddAsync(wishlistItem, ct);
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<IEnumerable<WishlistItemResponse>> GetUserWishlistAsync(string userId, CancellationToken ct)
    {
        var Wish = await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistItemResponse(
                w.Id,
                w.ProductId,
                w.Product.Name,
                w.Product.Price,
                w.Product.Stock > 0 && w.Product.IsActive,
                w.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? w.Product.Images.First(i => i.IsPrimary).Url
                    : null,
                w.Product.ProductCategories.Select(pc => pc.Category.Name).FirstOrDefault(),
                w.Product.Reviews.Any() ? Math.Round(w.Product.Reviews.Average(r => r.Rating), 1) : 0,
                w.Product.Reviews.Count
            ))
            .ToListAsync(ct);

        return Wish;
    }

    public async Task<Result> RemoveFromWishListAsync(int productId, string userId, CancellationToken ct)
    {
        var wishlistItem = await _context.Wishlists
        .SingleOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, ct);

        if (wishlistItem is null)
            return Result.Failure(WishListErrors.ItemNotInWishlist);

        _context.Wishlists.Remove(wishlistItem);
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }
}
