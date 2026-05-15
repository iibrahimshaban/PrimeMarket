namespace PrimeMarket.Entities;

public class Category : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public ICollection<ProductCategory> ProductCategories { get; set; } = [];
}
