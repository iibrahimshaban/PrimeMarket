namespace PrimeMarket.Persistence.EntityConfiguration;

public class CategoryConfiguration : AuditableEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
