using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Cart;
using PrimeMarket.Helpers;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    private string UserId => httpContextAccessor.HttpContext!.User.GetUserId()!;

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await cartService.GetCartAsync(UserId, ct);
        return result.IsSuccess ? Ok(result) : result.ToProblem();
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToCart(int productId, [FromBody] CartRequest request, CancellationToken ct)
    {
        var result = await cartService.AddToCartAsync(productId, UserId, request.Quantity, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] CartRequest request, CancellationToken ct)
    {
        var result = await cartService.UpdateQuantityAsync(cartItemId, UserId, request.Quantity, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId, CancellationToken ct)
    {
        var result = await cartService.RemoveFromCartAsync(cartItemId, UserId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
