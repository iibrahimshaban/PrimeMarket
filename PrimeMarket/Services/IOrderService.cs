using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;

namespace PrimeMarket.Services;

public interface IOrderService
{
    Task<Result<PlaceOrderResponse>> PlaceOrderAsync(string userId, PlaceOrderRequest request);
    Task<Result<PromoCodeValidationResponse>> ValidatePromoCodeAsync(string code, decimal cartTotal);
}
