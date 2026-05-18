using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Products;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService _productService) : ControllerBase
{
    private readonly IProductService productService = _productService;

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilter requestFilter, CancellationToken cancellationToken)
    {
        var products = await productService.GetAllProductsAsync(requestFilter, cancellationToken);

        return Ok(products);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterRequest request)
    {
        var result = await productService.GetFilteredProductsAsync(request);
        return Ok(result);
    }

    // -----------------------------------------------------------------------
    
    [HttpGet("details/{id:int}")]
    public async Task<IActionResult> GetCustomerProduct(int id)
    {
        var result = await productService.GetProductByIdForCustomerAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetProductByIdAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
    {

        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await productService.CreateProductAsync(request, sellerId);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await productService.UpdateProductAsync(id, request, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await productService.DeleteProductAsync(id, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPost("{id:int}/images")]
    [Authorize]
    public async Task<IActionResult> AddImage(int id, IFormFile image)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await productService.AddImageAsync(id, image, sellerId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpDelete("{id:int}/images/{imageId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await productService.DeleteImageAsync(id, imageId, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPut("{id:int}/images/{imageId:int}/set-primary")]
    [Authorize]
    public async Task<IActionResult> SetPrimaryImage(int id, int imageId)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await productService.SetPrimaryImageAsync(id, imageId, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
