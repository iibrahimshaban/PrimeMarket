using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.PromoCodes;
using PrimeMarket.Services;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PromoCodeController(IPromoCodeService promoCodeService) : ControllerBase
{
    private readonly IPromoCodeService _promoCodeService = promoCodeService;

    [HttpGet("All")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/orders")]
    public async Task<IActionResult> GetOrders(int id, CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.GetOrdersByPromoCodeAsync(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.CreateAsync(request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.UpdateAsync(id, request, cancellationToken);;

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
