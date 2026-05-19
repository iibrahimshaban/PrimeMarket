namespace PrimeMarket.Contracts.Inventory
{
    public record AdjustStockResponse(
        int ProductId,
        string ProductName,
        int PreviousStock,
        int QuantityChange,
        int NewStock
    );
}
