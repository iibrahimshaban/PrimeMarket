using PrimeMarket.Contracts.Brand;

namespace PrimeMarket.Services;

public interface IBrandService
{
    Task<List<BrandResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<BrandDetailsResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
}
