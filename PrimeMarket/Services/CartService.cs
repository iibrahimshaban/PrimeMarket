using PrimeMarket.Contracts.Cart;
using PrimeMarket.Errors;

namespace PrimeMarket.Services;

public class CartService(ApplicationDbContext context) : ICartService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<CartResponse>> GetCartAsync(string userId, CancellationToken ct)
    {
        var items = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemResponse(
                c.Id,
                c.ProductId,
                c.Product.Name,
                c.Product.Price,
                c.Quantity,
                c.Product.Price * c.Quantity,
                c.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? c.Product.Images.First(i => i.IsPrimary).Url
                    : null,
                c.Product.Stock > 0 && c.Product.IsActive,
                c.Product.Stock
            ))
            .ToListAsync(ct);

        var response = new CartResponse(
            items,
            items.Sum(i => i.Subtotal),
            items.Sum(i => i.Quantity)
        );

        return Result.Success(response);
    }

    public async Task<Result> AddToCartAsync(int productId, string userId, int quantity, CancellationToken ct)
    {
        if (quantity < 1)
            return Result.Failure(CartErrors.InvalidQuantity);

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive, ct);

        if (product is null)
            return Result.Failure(CartErrors.ProductNotFound);

        if (product.Stock < quantity)
            return Result.Failure(CartErrors.InsufficientStock);

        var existing = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId, ct);

        if (existing is not null)
            return Result.Failure(CartErrors.AlreadyInCart);

        _context.CartItems.Add(new CartItem
        {
            UserId = userId,
            ProductId = productId,
            Quantity = quantity
        });

        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> UpdateQuantityAsync(int cartItemId, string userId, int quantity, CancellationToken ct)
    {
        if (quantity < 1)
            return Result.Failure(CartErrors.InvalidQuantity);

        var item = await _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId, ct);

        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        if (item.Product.Stock < quantity)
            return Result.Failure(CartErrors.InsufficientStock);

        item.Quantity = quantity;
        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RemoveFromCartAsync(int cartItemId, string userId, CancellationToken ct)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId, ct);

        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}
