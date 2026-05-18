using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Categories;
using PrimeMarket.Services;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService _categoryService) : ControllerBase
{
    private readonly ICategoryService categoryService = _categoryService;

    // -----------------------------------------------------------------------

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAllCategoryAsync();
        return Ok(categories);
    }

    // -----------------------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await categoryService.GetByIdCategoryAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var result = await categoryService.CreateCategoryAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        var result = await categoryService.UpdateCategoryAsync(id, request);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    // -----------------------------------------------------------------------

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categoryService.DeleteCategoryAsync(id);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}