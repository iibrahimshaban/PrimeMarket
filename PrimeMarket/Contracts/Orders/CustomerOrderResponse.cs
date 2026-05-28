namespace PrimeMarket.Contracts.Orders
{
    public record CustomerOrderResponse(
        int OrderId,
        DateTime OrderDate,
        OrderStatus Status,
        decimal TotalAmount,
        string PaymentMethod,
        OrderAddressResponse Address,
        IReadOnlyList<CustomerOrderItemResponse> Items
    );
}
