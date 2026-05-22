namespace PrimeMarket.Contracts.Orders
{
    public record SellerOrderItemResponse(
        int ProductId,
        string ProductName,
        string ProductThumbnail,
        int Quantity,
        decimal UnitPrice,
        decimal Subtotal
    );
}
