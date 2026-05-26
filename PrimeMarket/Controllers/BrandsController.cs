using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Brand;
using PrimeMarket.Helpers;
using PrimeMarket.Services;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandsController(IBrandService brandService) : ControllerBase
{
    private readonly IBrandService _brandService = brandService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _brandService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _brandService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("register")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> BecomeSeller([FromForm] BecomeSelerRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId()!;
        var result = await _brandService.BecomeSelerAsync(request, userId, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpGet("status")]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId()!;
        var result = await _brandService.GetStatusAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpGet("seller-requests")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetAllSellerRequests(CancellationToken cancellationToken)
    {
        var sellerRequests = await _brandService.GetAllSellerRequestsAsync(cancellationToken);
        return Ok(sellerRequests);
    }
    [HttpPut("{brandId}/approve")]
    public async Task<IActionResult> Approve(int brandId, CancellationToken ct)
    {
        var result = await _brandService.ApproveSellerRequestAsync(brandId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{brandId}/reject")]
    public async Task<IActionResult> Reject(int brandId, CancellationToken ct)
    {
        var result = await _brandService.RejectSellerRequestAsync(brandId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
