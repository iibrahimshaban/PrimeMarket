using PrimeMarket.Contracts;
using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;

namespace PrimeMarket.Services;

public interface IOrderService
{
    Task<Result<PlaceOrderResponse>> PlaceOrderAsync(string userId, PlaceOrderRequest request);
    Task<Result<PromoCodeValidationResponse>> ValidatePromoCodeAsync(string code, decimal cartTotal);
    Task<PaginationList<SellerOrderResponse>> GetSellerOrdersAsync(string sellerId,RequestFilter filter,CancellationToken cancellationToken);
    Task<Result<SellerOrderResponse>> GetSellerOrderByIdAsync(string sellerId, int orderId);
    Task<Result> UpdateOrderStatusAsync(string sellerId, int orderId, OrderStatus newStatus);
}
