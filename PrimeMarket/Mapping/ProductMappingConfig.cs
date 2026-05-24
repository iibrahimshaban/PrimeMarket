using PrimeMarket.Contracts.Products;

namespace PrimeMarket.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
           .Map(dest => dest.InStock,
               src => src.Stock > 0)

           .Map(dest => dest.Thumbnail,
                src => src.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.Url)
                    .FirstOrDefault() ?? string.Empty)

            .Map(dest => dest.Images,
                src => (IReadOnlyList<ProductImageResponse>)src.Images
                    .Select(i => new ProductImageResponse(i.Id, i.Url, i.IsPrimary))
                    .OrderByDescending(i => i.IsPrimary)
                    .ToList())

           .Map(dest => dest.SellerName,
               src => $"{src.Seller.FirstName} {src.Seller.LastName}")

           .Map(dest => dest.Categories,
               src => (IReadOnlyList<string>)src.ProductCategories
                   .Select(pc => pc.Category.Name)
                   .ToList())

           .Map(dest => dest.AverageRating,
               src => src.Reviews.Count == 0
                   ? 0.0
                   : Math.Round(src.Reviews.Average(r => (double)r.Rating), 1))

           .Map(dest => dest.ReviewCount,
               src => src.Reviews.Count)

           .Map(dest => dest.OrderCount,
               src => src.OrderItems.Sum(oi => oi.Quantity));

        config.NewConfig<CreateProductRequest, Product>()
            .Map(dest => dest.ProductCategories, src => src.CategoryIds.Distinct().Select(id => new ProductCategory { CategoryId = id }));

        config.NewConfig<UpdateProductRequest, Product>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.SellerId)
            .Ignore(dest => dest.Images)
            .Ignore(dest => dest.ProductCategories)
            .Ignore(dest => dest.Stock);

        config.NewConfig<Product, ProductCustomerResponse>()
            .ConstructUsing(src => new ProductCustomerResponse(
                src.Id,
                src.Name,
                src.Price,
                src.Stock > 0 && src.IsActive,
                src.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? src.Images.First(i => i.IsPrimary).Url
                    : null,
                src.ProductCategories.Select(pc => pc.Category.Name).FirstOrDefault(),
                src.Reviews.Any() ? Math.Round(src.Reviews.Average(r => r.Rating), 1) : 0,
                src.Reviews.Count
            ));

        config.NewConfig<Review, ProductReviewResponse>()
            .Map(dest => dest.UserName, src => src.User.FirstName + " " + src.User.LastName)
            .Map(dest => dest.UserAvatar, src => src.User.ProfilePictureUrl);

        config.NewConfig<Product, ProductDetailCustomerResponse>()
            .ConstructUsing(src => new ProductDetailCustomerResponse(
                src.Id,
                src.Name,
                src.Description,
                src.Price,
                src.Stock,
                src.Seller.FirstName + " " + src.Seller.LastName,
                src.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? src.Images.First(i => i.IsPrimary).Url
                    : null,
                src.Images.Select(i => i.Url).ToList(),
                src.ProductCategories.Select(pc => pc.Category.Name).FirstOrDefault(),
                src.Reviews.Any()
                    ? Math.Round(src.Reviews.Average(r => r.Rating), 1)
                    : 0,
                src.Reviews.Count,
                src.Reviews.Adapt<List<ProductReviewResponse>>()
            ));

        config.NewConfig<Product, SellerProductResponse>()
            .ConstructUsing(src => new SellerProductResponse(
                src.Id,
                src.Name,
                src.Price,
                src.Stock,
                src.IsActive,

                src.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.Url)
                    .FirstOrDefault(),

                src.Reviews.Any()
                    ? Math.Round(src.Reviews.Average(r => r.Rating), 1)
                    : 0,

                src.Reviews.Count,

                src.ProductCategories
                    .Select(pc => pc.Category.Name)
                    .ToList(),
                src.CreatedOn
            ));
    }
}
