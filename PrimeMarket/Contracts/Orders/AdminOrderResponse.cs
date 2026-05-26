namespace PrimeMarket.Contracts.Orders
{
    public record AdminOrderResponse(
        int OrderId,
        string CustomerUserName,
        string CustomerEmail,
        DateTime OrderDate,
        OrderStatus Status,
        decimal TotalAmount,
        string PaymentMethod,
        OrderAddressResponse Address,
        IReadOnlyList<AdminOrderItemResponse> Items
    );
}
