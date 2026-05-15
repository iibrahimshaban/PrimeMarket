namespace PrimeMarket.Entities;

public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public bool IsDefault { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = [];
}
