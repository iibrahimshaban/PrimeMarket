using PrimeMarket.Contracts.Reviews;

namespace PrimeMarket.Services;

public class ReviewService(ApplicationDbContext context) : IReviewService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result> AddReviewAsync(int productId, string userId, AddReviewRequest request, CancellationToken ct = default)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId && p.IsActive, ct);
        if (!productExists)
            return Result.Failure(ProductError.ProductNotFound);

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}
