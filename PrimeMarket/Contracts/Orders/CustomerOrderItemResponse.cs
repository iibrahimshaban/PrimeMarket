namespace PrimeMarket.Contracts.Orders
{
    public record CustomerOrderItemResponse(
        int ProductId,
        string ProductName,
        string ProductThumbnail,
        int Quantity,
        decimal UnitPrice,
        decimal Subtotal,
        string SellerUserName
    );
}
