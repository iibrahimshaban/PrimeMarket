using PrimeMarket.Contracts.Brand;

namespace PrimeMarket.Services;

public interface IBrandService
{
    Task<List<BrandResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<BrandDetailsResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result> BecomeSelerAsync(BecomeSelerRequest request, string userId, CancellationToken cancellationToken);
    Task<Result> GetStatusAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BecomeSellerResponse>> GetAllSellerRequestsAsync(CancellationToken cancellationToken = default);
    Task<Result> ApproveSellerRequestAsync(int brandId, CancellationToken cancellationToken = default);
    Task<Result> RejectSellerRequestAsync(int brandId, CancellationToken cancellationToken = default);
    Task<Result<SellerRequestDetailsResponse>> GetSellerRequestDetailsAsync(int brandId, CancellationToken cancellationToken = default);
}
