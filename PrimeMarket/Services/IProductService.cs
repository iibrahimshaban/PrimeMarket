using PrimeMarket.Contracts;
using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Products;

namespace PrimeMarket.Services;

public interface IProductService
{
    Task<PaginationList<ProductCustomerResponse>> GetAllProductsAsync(RequestFilter filter, CancellationToken cancellationToken);
    Task<PaginatedResponse<ProductResponse>> GetFilteredProductsAsync(ProductFilterRequest request);
    Task<Result<ProductDetailCustomerResponse>> GetProductByIdForCustomerAsync(int id);
    Task<Result<ProductResponse>> GetProductByIdAsync(int id);
    Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, string sellerId);
    Task<Result> UpdateProductAsync(int id, UpdateProductRequest request, string sellerId);
    Task<Result> DeleteProductAsync(int id, string sellerId);

    Task<Result<ProductImageResponse>> AddImageAsync(int productId, IFormFile image, string sellerId);
    Task<Result> DeleteImageAsync(int productId, int imageId, string sellerId);
    Task<Result> SetPrimaryImageAsync(int productId, int imageId, string sellerId);
}
