using PrimeMarket.Contracts.Products;

namespace PrimeMarket.Services;

public interface IProductService
{
   Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
}
