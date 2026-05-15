namespace PrimeMarket.Persistence.EntityConfiguration;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
    }
}
