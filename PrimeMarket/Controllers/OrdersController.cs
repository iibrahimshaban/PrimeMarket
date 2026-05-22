using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.PromoCodes;
using PrimeMarket.Helpers;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private string UserId => User.GetUserId()!;
    private readonly IOrderService _orderService = orderService;

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var result = await _orderService.PlaceOrderAsync(UserId, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("validate-promo")]
    public async Task<IActionResult> ValidatePromo([FromBody] ValidatePromoCodeRequest request)
    {
        var result = await _orderService.ValidatePromoCodeAsync(request.Code, request.CartTotal);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }


    // -----------------------------------------------------------------------

    [HttpGet("seller")]
    [Authorize]
    public async Task<IActionResult> GetSellerOrders(
        [FromQuery] RequestFilter filter,
        CancellationToken cancellationToken)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await orderService.GetSellerOrdersAsync(
            sellerId!,
            filter,
            cancellationToken);

        return Ok(result);
    }


    // -----------------------------------------------------------------------

    [HttpGet("seller/{orderId:int}")]
    [Authorize]
    public async Task<IActionResult> GetSellerOrderById(int orderId)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await orderService.GetSellerOrderByIdAsync(
            sellerId!,
            orderId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPut("seller/{orderId:int}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateOrderStatus(
        int orderId,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await orderService.UpdateOrderStatusAsync(
            sellerId!,
            orderId,
            request.Status);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
