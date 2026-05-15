namespace PrimeMarket.Persistence.EntityConfiguration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // One review per user per product
        builder.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();

        builder.Property(x => x.Rating).IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "Rating >= 1 AND Rating <= 5"));
    }
}
