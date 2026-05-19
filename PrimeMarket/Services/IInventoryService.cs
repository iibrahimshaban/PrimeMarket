using PrimeMarket.Contracts.Inventory;

namespace PrimeMarket.Services
{
    public interface IInventoryService
    {
        Task<Result<StockSummaryResponse>> GetStockSummaryAsync(int productId);
        Task<Result<AdjustStockResponse>> AdjustStockAsync(int productId, AdjustStockRequest request, string? sellerId);
    }
}
