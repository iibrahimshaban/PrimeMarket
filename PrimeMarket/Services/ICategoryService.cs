using PrimeMarket.Contracts.Categories;

namespace PrimeMarket.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync();
        Task<Result<CategoryResponse>> GetByIdCategoryAsync(int id);
        Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<Result> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
        Task<Result> DeleteCategoryAsync(int id);
    }
}
