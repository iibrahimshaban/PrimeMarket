namespace PrimeMarket.Persistence.EntityConfiguration;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.ProviderToken)
            .IsRequired()
            .HasMaxLength(500);

    }
}
