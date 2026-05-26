using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
}
