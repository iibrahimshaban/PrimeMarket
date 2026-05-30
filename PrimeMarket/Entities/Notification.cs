namespace PrimeMarket.Entities;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // "order", "broadcast", "general"
    public bool IsRead { get; set; }
    public DateTime? ReadOn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = default!;
}
