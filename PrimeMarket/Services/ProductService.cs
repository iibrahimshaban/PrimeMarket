using PrimeMarket.Contracts.Products;
using PrimeMarket.Errors;

namespace PrimeMarket.Services;

public class ProductService(ApplicationDbContext context) : IProductService
{
    private readonly ApplicationDbContext _cotext = context;

    public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
        var products = await context.Products
             .Where(p => p.IsActive && p.Stock > 0)
             .Include(p => p.Seller)
             .Include(p => p.Images)
             .Include(p => p.Reviews)
             .Include(p => p.OrderItems)
             .Include(p => p.ProductCategories)
                 .ThenInclude(pc => pc.Category)
             .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
                 .ThenByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
             .ToListAsync();         

        return products.Adapt<List<ProductResponse>>();  
    }
}
