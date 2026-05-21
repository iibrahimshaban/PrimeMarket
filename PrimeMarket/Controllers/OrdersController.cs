using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
}
