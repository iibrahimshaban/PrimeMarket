namespace PrimeMarket.Persistence.EntityConfiguration;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);
    }
}
