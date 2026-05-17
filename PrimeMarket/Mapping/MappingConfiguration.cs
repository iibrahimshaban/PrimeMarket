using PrimeMarket.Contracts.Products;

namespace PrimeMarket.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
           .Map(dest => dest.InStock,
               src => src.Stock > 0)

           .Map(dest => dest.PrimaryImageUrl,
               src => src.Images
                   .Where(i => i.IsPrimary)
                   .Select(i => i.Url)
                   .FirstOrDefault() ?? string.Empty)

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
    }
}
