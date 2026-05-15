namespace PrimeMarket.Persistence.EntityConfiguration;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
    }
}
