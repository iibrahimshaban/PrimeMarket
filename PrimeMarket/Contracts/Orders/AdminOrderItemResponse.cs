namespace PrimeMarket.Contracts.Orders
{
    public record AdminOrderItemResponse(
        int ProductId,
        string ProductName,
        string ProductThumbnail,
        int Quantity,
        decimal UnitPrice,
        decimal Subtotal,
        string SellerUserName
    );
}
