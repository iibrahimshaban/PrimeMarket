using PrimeMarket.Contracts.PromoCodes;

namespace PrimeMarket.Services;

public interface IPromoCodeService
{
    Task<List<PromoCodeResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<PromoCodeDetailsResponse>> GetOrdersByPromoCodeAsync(int promoCodeId, CancellationToken cancellationToken);
    Task<Result> CreateAsync(CreatePromoCodeRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int id, UpdatePromoCodeRequest request, CancellationToken cancellationToken);
}
