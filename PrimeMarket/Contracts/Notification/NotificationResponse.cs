namespace PrimeMarket.Contracts.Notification;

public record NotificationResponse(
    int Id,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    DateTime? ReadOn,
    DateTime CreatedOn
);
