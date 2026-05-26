using PrimeMarket.Contracts.Brand;

namespace PrimeMarket.Services;

public interface IBrandService
{
    Task<List<BrandResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<BrandDetailsResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result> BecomeSelerAsync(BecomeSelerRequest request, string userId, CancellationToken cancellationToken);
}
