namespace PrimeMarket.Persistence.EntityConfiguration;

public class OrderConfiguration : AuditableEntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .IsRequired();

        // Nullable FK — explicit to control OnDelete behavior
        builder.HasOne(x => x.PromoCode)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.PromoCodeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
