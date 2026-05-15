namespace PrimeMarket.Persistence.EntityConfiguration;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(x => x.Street).IsRequired().HasMaxLength(200);
        builder.Property(x => x.City).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(100);
    }
}
