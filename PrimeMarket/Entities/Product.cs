namespace PrimeMarket.Entities;

public class Product : AuditableEntity
{
    public int Id { get; set; }
    public string SellerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? BrandName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser Seller { get; set; } = null!;
    public ICollection<ProductCategory> ProductCategories { get; set; } = [];
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Wishlist> Wishlists { get; set; } = [];
}
