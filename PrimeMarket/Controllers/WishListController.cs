using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Helpers;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishListController(IHttpContextAccessor httpContextAccessor, IWishListService wishlistService) : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IWishListService _wishlistService = wishlistService;

    [HttpGet("")]
    public async Task<IActionResult> GetWishlist( CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;
        var result = await _wishlistService.GetUserWishlistAsync(userId, ct);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("{ProductId}")]
    public async Task<IActionResult> AddToWishlist([FromRoute]int productId, CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;
        var result = await _wishlistService.AddToWishListAsync(productId, userId, ct);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpDelete("{ProductId}")]
    public async Task<IActionResult> RemoveFromWishlist([FromRoute]int productId, CancellationToken ct)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserId()!;
        var result = await _wishlistService.RemoveFromWishListAsync(productId, userId, ct);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
