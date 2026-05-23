using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;

namespace PrimeMarket.Services;

public interface IOrderService
{
    Task<Result<GetOrderResponse>> GetOrderByIdAsync(string userId, string id);
    Task<Result<PlaceOrderResponse>> PlaceOrderAsync(string userId, PlaceOrderRequest request);
    Task<Result<PromoCodeValidationResponse>> ValidatePromoCodeAsync(string code, decimal cartTotal);
}
