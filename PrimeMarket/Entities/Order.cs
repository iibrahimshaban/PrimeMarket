namespace PrimeMarket.Entities;

public class Order : AuditableEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int AddressId { get; set; }
    public int? PromoCodeId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentType PaymentMethod { get; set; }
    public string? PaymentRef { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public PromoCode? PromoCode { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}
