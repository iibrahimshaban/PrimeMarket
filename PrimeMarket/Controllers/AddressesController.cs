using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Address;
using PrimeMarket.Helpers;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AddressesController(IAddressService addressService) : ControllerBase
{
    private readonly IAddressService _addressService = addressService;
    private string UserId => User.GetUserId()!;

    [HttpGet("")]
    public async Task<IActionResult> GetMyAddresses()
    {
        var result = await _addressService.GetUserAddressesAsync(UserId);
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request)
    {
        var result = await _addressService.AddAddressAsync(UserId, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
