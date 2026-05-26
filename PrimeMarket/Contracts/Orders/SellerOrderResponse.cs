namespace PrimeMarket.Contracts.Orders
{
    public record SellerOrderResponse(
        int OrderId,
        string CustomerName,
        string CustomerEmail,
        DateTime OrderDate,
        OrderStatus Status,
        decimal TotalAmount,
        string PaymentMethod,
        OrderAddressResponse Address,
        IReadOnlyList<SellerOrderItemResponse> Items
    );
}
