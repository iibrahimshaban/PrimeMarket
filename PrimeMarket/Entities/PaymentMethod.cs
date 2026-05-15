namespace PrimeMarket.Entities;

public class PaymentMethod
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public PaymentType Type { get; set; }
    public string ProviderToken { get; set; } = null!;
    public bool IsDefault { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
