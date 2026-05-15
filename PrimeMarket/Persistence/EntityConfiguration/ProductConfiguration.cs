namespace PrimeMarket.Persistence.EntityConfiguration;

public class ProductConfiguration : AuditableEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Seller)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
