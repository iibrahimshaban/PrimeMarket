namespace PrimeMarket.Persistence.EntityConfiguration;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
    }
}
