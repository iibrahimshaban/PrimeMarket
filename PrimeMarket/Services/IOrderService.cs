using PrimeMarket.Contracts;
using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;

namespace PrimeMarket.Services;

public interface IOrderService
{
    Task<Result<GetOrderResponse>> GetOrderByIdAsync(string userId, string id);
    Task<Result<PlaceOrderResponse>> PlaceOrderAsync(string userId, PlaceOrderRequest request);
    Task<Result<PromoCodeValidationResponse>> ValidatePromoCodeAsync(string code, decimal cartTotal);
    Task<PaginationList<CustomerOrderResponse>> GetCustomerOrdersAsync(string customerId, RequestFilter filter,CancellationToken cancellationToken);
    Task<PaginationList<SellerOrderResponse>> GetSellerOrdersAsync(string sellerId,RequestFilter filter,CancellationToken cancellationToken);
    Task<PaginationList<AdminOrderResponse>> GetAdminOrdersAsync(RequestFilter filter,CancellationToken cancellationToken);
    Task<Result<SellerOrderResponse>> GetSellerOrderByIdAsync(string sellerId, int orderId);
    Task<Result> UpdateOrderStatusAsync(string userId, int orderId, OrderStatus newStatus);
}
