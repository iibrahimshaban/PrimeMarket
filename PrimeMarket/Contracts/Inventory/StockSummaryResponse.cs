namespace PrimeMarket.Contracts.Inventory
{
    public record StockSummaryResponse(
        int ProductId,
        string ProductName,
        int CurrentStock,
        bool InStock
    );
}
