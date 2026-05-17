using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Services;

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
    
}
