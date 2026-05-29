using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Inventory;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/products/{productId:int}/inventory")]
[ApiController]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    private readonly IInventoryService _inventoryService = inventoryService;

    [HttpGet]
    public async Task<IActionResult> GetStockSummary(int productId)
    {
        var result = await _inventoryService.GetStockSummaryAsync(productId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpPost("adjust")]
    [Authorize(Roles = DefaultRoles.Seller)]
    public async Task<IActionResult> AdjustStock(int productId,[FromBody] AdjustStockRequest request)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _inventoryService.AdjustStockAsync(productId, request, sellerId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
}