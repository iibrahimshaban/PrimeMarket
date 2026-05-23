namespace PrimeMarket.Contracts.Orders;

public record GetOrderResponse(
    int Id,
    decimal TotalAmount,
    decimal DiscountAmount,
    string Address,
    DateTime EstimatedDelivery,
    List<OrderItemResponse> Items
);
