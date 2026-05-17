using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Products;
using PrimeMarket.Services;
using System.Security.Claims;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService productService = productService;
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var products = await productService.GetAllProductsAsync();
        return Ok(products);
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
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
    {

        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? DefaultUsers.UserId;

        var result = await productService.CreateProductAsync(request, sellerId);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPut("{id:int}")]
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? DefaultUsers.UserId;

        var result = await productService.UpdateProductAsync(id, request, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpDelete("{id:int}")]
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? DefaultUsers.UserId;

        var result = await productService.DeleteProductAsync(id, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPost("{id:int}/images")]
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> AddImage(int id, IFormFile image)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? DefaultUsers.UserId;

        var result = await productService.AddImageAsync(id, image, sellerId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpDelete("{id:int}/images/{imageId:int}")]
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? DefaultUsers.UserId;

        var result = await productService.DeleteImageAsync(id, imageId, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    // -----------------------------------------------------------------------

    [HttpPut("{id:int}/images/{imageId:int}/set-primary")]
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> SetPrimaryImage(int id, int imageId)
    {
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? DefaultUsers.UserId;

        var result = await productService.SetPrimaryImageAsync(id, imageId, sellerId);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
