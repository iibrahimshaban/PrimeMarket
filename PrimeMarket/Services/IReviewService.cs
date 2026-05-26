using PrimeMarket.Contracts.Reviews;

namespace PrimeMarket.Services;

public interface IReviewService
{
    Task<Result> AddReviewAsync(int productId, string userId, AddReviewRequest request, CancellationToken ct = default);
}
