using PrimeMarket.Contracts;
using PrimeMarket.Contracts.Common;
using PrimeMarket.Contracts.Products;
using PrimeMarket.Errors;

namespace PrimeMarket.Services;

public class ProductService(ApplicationDbContext context, ICloudinaryService cloudinaryService) : IProductService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<PaginationList<ProductCustomerResponse>> GetAllProductsAsync(RequestFilter filter,CancellationToken cancellationToken)
    {
        // 1. Base query — IQueryable, nothing hits DB yet
        var query = _context.Products
            .Where(p => p.IsActive && p.Stock > 0)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .AsQueryable();

        // 2. Search
        if (!string.IsNullOrWhiteSpace(filter.SearchValue))
        {
            var search = filter.SearchValue.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(search));
        }

        // 3. Sort — safe column whitelist (never interpolate raw column names)
        var isDesc = filter.SortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase);

        query = filter.SortColumn?.ToLower() switch
        {
            "name" => isDesc ? query.OrderByDescending(p => p.Name)
                                 : query.OrderBy(p => p.Name),

            "price" => isDesc ? query.OrderByDescending(p => p.Price)
                                 : query.OrderBy(p => p.Price),

            "stock" => isDesc ? query.OrderByDescending(p => p.Stock)
                                 : query.OrderBy(p => p.Stock),

            "rating" => isDesc
                            ? query.OrderByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
                            : query.OrderBy(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0),

            // Default: most ordered → highest rated (your original logic)
            _ => query
                    .OrderByDescending(p => p.OrderItems.Sum(oi => (int?)oi.Quantity) ?? 0)
                    .ThenByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
        };

        // 4. Project to DTO *before* pagination count/skip/take
        //    This keeps the SELECT lean — no unnecessary columns
        var projected = query.ProjectToType<ProductCustomerResponse>();

        // 5. Paginate (hits DB twice: COUNT + paged SELECT)
        return await PaginationList<ProductCustomerResponse>.CreateAsync(
            projected,
            filter.PageNumber,
            filter.PageSize
        );
    }

    public async Task<Result<IEnumerable<ProductCustomerResponse>>> GetProductByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Where(p => p.IsActive && p.Stock > 0 && p.ProductCategories.Any(pc => pc.CategoryId == categoryId))
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .ToListAsync(cancellationToken);

        if (product is null)
            return Result.Failure<IEnumerable<ProductCustomerResponse>>(ProductError.ProductNotFound);

        return Result.Success(product.Adapt<IEnumerable<ProductCustomerResponse>>());
    }
    public async Task<PaginationList<SellerProductResponse>> GetSellerProductsAsync(
        string sellerId,
        RequestFilter filter,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Where(p => p.SellerId == sellerId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchValue))
        {
            var search = filter.SearchValue.Trim().ToLower();

            query = query.Where(p =>
                p.Name.ToLower().Contains(search));
        }

        var isDesc = filter.SortDirection.Equals(
            "DESC",
            StringComparison.OrdinalIgnoreCase);

        query = filter.SortColumn?.ToLower() switch
        {
            "name" => isDesc
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            "price" => isDesc
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

            "stock" => isDesc
                ? query.OrderByDescending(p => p.Stock)
                : query.OrderBy(p => p.Stock),

            "rating" => isDesc
                ? query.OrderByDescending(
                    p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
                : query.OrderBy(
                    p => p.Reviews.Average(r => (double?)r.Rating) ?? 0),

            "createdon" => isDesc
                ? query.OrderByDescending(p => p.CreatedOn)
                : query.OrderBy(p => p.CreatedOn),

            _ => query.OrderByDescending(p => p.CreatedOn)
        };

        var projected = query.ProjectToType<SellerProductResponse>();

        return await PaginationList<SellerProductResponse>.CreateAsync(
            projected,
            filter.PageNumber,
            filter.PageSize
        );
    }
    //---------------------------------------------------------------------------------------------------

    public async Task<Result<ProductDetailCustomerResponse>> GetProductByIdForCustomerAsync(int id)
    {
        var product = await _context.Products
            .Where(p => p.Id == id && p.IsActive)
            .Include(p => p.Seller)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync();

        if (product is null)
            return Result.Failure<ProductDetailCustomerResponse>(ProductError.ProductNotFound);

        return Result.Success(product.Adapt<ProductDetailCustomerResponse>());
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Where(p => p.Id == id && p.IsActive)
            .Include(p => p.Seller)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync();

        if (product is null)
            return Result.Failure<ProductResponse>(ProductError.ProductNotFound);

        return Result.Success(product.Adapt<ProductResponse>());
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, string sellerId)
    {
        var categoryIds = request.CategoryIds.Distinct().ToList();
        var existingCategoryCount = await _context.Categories
            .CountAsync(c => categoryIds.Contains(c.Id));

        if (existingCategoryCount != categoryIds.Count)
            return Result.Failure<ProductResponse>(ProductError.InvalidCategory);

        var product = request.Adapt<Product>(); 
        product.SellerId = sellerId; 
        product.IsActive = true;

        var safeProductName = request.Name.ToLower().Replace(" ", "-");

        var primaryImageUrl = await _cloudinaryService.UploadImageAsync(
            request.PrimaryImage,
            folder: "products",
            publicId: $"products/{safeProductName}-primary-{Guid.NewGuid():N}"
        );

        product.Images.Add(new ProductImage
        {
            Url = primaryImageUrl,
            IsPrimary = true
        });

        if (request.ExtraImages is { Count: > 0 })
        {
            foreach (var (file, index) in request.ExtraImages.Select((f, i) => (f, i)))
            {
                var extraUrl = await _cloudinaryService.UploadImageAsync(
                    file,
                    folder: "products",
                    publicId: $"products/{safeProductName}-extra-{index}-{Guid.NewGuid():N}"
                );

                product.Images.Add(new ProductImage
                {
                    Url = extraUrl,
                    IsPrimary = false
                });
            }
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var created = await _context.Products
            .Where(p => p.Id == product.Id)
            .Include(p => p.Seller)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .FirstAsync();

        return Result.Success(created.Adapt<ProductResponse>());
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result> UpdateProductAsync(int id, UpdateProductRequest request, string sellerId)
    {
        var product = await _context.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return Result.Failure(ProductError.ProductNotFound);

        if (product.SellerId != sellerId)
            return Result.Failure(ProductError.UnauthorizedAction);

        var categoryIds = request.CategoryIds.Distinct().ToList();
        var existingCategoryCount = await _context.Categories
            .CountAsync(c => categoryIds.Contains(c.Id));

        if (existingCategoryCount != categoryIds.Count)
            return Result.Failure(ProductError.InvalidCategory);

        request.Adapt(product);

        _context.ProductCategories.RemoveRange(product.ProductCategories);
        await _context.ProductCategories.AddRangeAsync(
            categoryIds.Select(cId => new ProductCategory { ProductId = id, CategoryId = cId })
        );

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result> DeleteProductAsync(int id, string sellerId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return Result.Failure(ProductError.ProductNotFound);

        if (product.SellerId != sellerId)
            return Result.Failure(ProductError.UnauthorizedAction);

        product.IsActive = false;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result<ProductImageResponse>> AddImageAsync(int productId, IFormFile image, string sellerId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
            return Result.Failure<ProductImageResponse>(ProductError.ProductNotFound);

        if (product.SellerId != sellerId)
            return Result.Failure<ProductImageResponse>(ProductError.UnauthorizedAction);

        var safeProductName = product.Name.ToLower().Replace(" ", "-");

        var imageUrl = await _cloudinaryService.UploadImageAsync(
            image,
            folder: "products",
            publicId: $"products/{safeProductName}-extra-{Guid.NewGuid():N}"
        );

        var productImage = new ProductImage
        {
            ProductId = productId,
            Url = imageUrl,
            IsPrimary = false
        };

        _context.ProductImages.Add(productImage);
        await _context.SaveChangesAsync();

        return Result.Success(productImage.Adapt<ProductImageResponse>());
    }

    //---------------------------------------------------------------------------------------------------
    public async Task<Result> DeleteImageAsync(int productId, int imageId, string sellerId)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
            return Result.Failure(ProductError.ProductNotFound);

        if (product.SellerId != sellerId)
            return Result.Failure(ProductError.UnauthorizedAction);

        var image = product.Images.FirstOrDefault(i => i.Id == imageId);

        if (image is null)
            return Result.Failure(ProductError.ImageNotFound);

        if (image.IsPrimary)
            return Result.Failure(ProductError.CannotDeletePrimaryImage);

        if (product.Images.Count == 1)
            return Result.Failure(ProductError.CannotDeleteOnlyImage);

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();

        await _cloudinaryService.DeleteImageByUrlAsync(image.Url);

        return Result.Success();
    }


    //---------------------------------------------------------------------------------------------------
    public async Task<Result> SetPrimaryImageAsync(int productId, int imageId, string sellerId)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
            return Result.Failure(ProductError.ProductNotFound);

        if (product.SellerId != sellerId)
            return Result.Failure(ProductError.UnauthorizedAction);

        var newPrimary = product.Images.FirstOrDefault(i => i.Id == imageId);

        if (newPrimary is null)
            return Result.Failure(ProductError.ImageNotFound);

        if (newPrimary.IsPrimary)
            return Result.Success();

        var currentPrimary = product.Images.FirstOrDefault(i => i.IsPrimary);
        if (currentPrimary is not null)
            currentPrimary.IsPrimary = false;

        newPrimary.IsPrimary = true;

        await _context.SaveChangesAsync();

        return Result.Success();
    }
}
