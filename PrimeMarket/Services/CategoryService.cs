using PrimeMarket.Contracts.Categories;
using PrimeMarket.Errors;
using System.Text.RegularExpressions;

namespace PrimeMarket.Services;

public class CategoryService(ApplicationDbContext context) : ICategoryService
{
    private readonly ApplicationDbContext _context = context;

    //-------------------------------------------------------------------------------------
    public async Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        return categories.Adapt<List<CategoryResponse>>();
    }

    //-------------------------------------------------------------------------------------
    public async Task<Result<CategoryResponse>> GetByIdCategoryAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return Result.Failure<CategoryResponse>(CategoryError.CategoryNotFound);

        return Result.Success(category.Adapt<CategoryResponse>());
    }

    //-------------------------------------------------------------------------------------
    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var exists = await _context.Categories
            .AnyAsync(c => c.Name == request.Name);

        if (exists)
            return Result.Failure<CategoryResponse>(CategoryError.CategoryAlreadyExists);

        var category = request.Adapt<Category>();
        category.Slug = GenerateSlug(request.Name);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Result.Success(category.Adapt<CategoryResponse>());
    }

    //-------------------------------------------------------------------------------------
    public async Task<Result> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return Result.Failure(CategoryError.CategoryNotFound);

        var exists = await _context.Categories
            .AnyAsync(c => c.Name == request.Name && c.Id != id);

        if (exists)
            return Result.Failure(CategoryError.CategoryAlreadyExists);

        request.Adapt(category);
        category.Slug = GenerateSlug(request.Name);


        await _context.SaveChangesAsync();

        return Result.Success();
    }

    //-------------------------------------------------------------------------------------
    public async Task<Result> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.ProductCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return Result.Failure(CategoryError.CategoryNotFound);

        if (category.ProductCategories.Any())
            return Result.Failure(CategoryError.CategoryInUse);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    //------------------------------------------------------------------------
    private static string GenerateSlug(string name)
    {
        return Regex.Replace(name.Trim().ToLower(), @"\s+", "-");
    }
}
