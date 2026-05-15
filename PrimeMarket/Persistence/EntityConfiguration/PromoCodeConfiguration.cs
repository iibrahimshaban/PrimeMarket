namespace PrimeMarket.Persistence.EntityConfiguration;

public class PromoCodeConfiguration : AuditableEntityConfiguration<PromoCode>
{
    public override void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.DiscountType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
    }
}
