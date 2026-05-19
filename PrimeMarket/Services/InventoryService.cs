using PrimeMarket.Contracts.Inventory;
using PrimeMarket.Errors;

namespace PrimeMarket.Services;

public class InventoryService(ApplicationDbContext context) : IInventoryService
{
    private readonly ApplicationDbContext _context = context;

    //---------------------------------------------------------------------------------------------------
    public async Task<Result<StockSummaryResponse>> GetStockSummaryAsync(int productId)
    {
        var product = await _context.Products
            .Where(p => p.Id == productId && p.IsActive)
            .Select(p => new StockSummaryResponse(
                p.Id,
                p.Name,
                p.Stock,
                p.Stock > 0))
            .FirstOrDefaultAsync();

        if (product is null)
            return Result.Failure<StockSummaryResponse>(InventoryError.ProductNotFound);

        return Result.Success(product);
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result<AdjustStockResponse>> AdjustStockAsync(
        int productId, AdjustStockRequest request, string? sellerId)
    {
        if (request.QuantityChange == 0)
            return Result.Failure<AdjustStockResponse>(InventoryError.InvalidQuantity);

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

        if (product is null)
            return Result.Failure<AdjustStockResponse>(InventoryError.ProductNotFound);

        if (product.SellerId != sellerId)
            return Result.Failure<AdjustStockResponse>(InventoryError.UnauthorizedAction);

        var previousStock = product.Stock;
        var newStock = previousStock + request.QuantityChange;

        if (newStock < 0)
            return Result.Failure<AdjustStockResponse>(InventoryError.InsufficientStock);

        product.Stock = newStock;
        await _context.SaveChangesAsync();

        return Result.Success(new AdjustStockResponse(
            product.Id,
            product.Name,
            previousStock,
            request.QuantityChange,
            newStock
        ));
    }
}