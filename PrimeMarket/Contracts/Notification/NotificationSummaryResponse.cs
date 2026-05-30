namespace PrimeMarket.Contracts.Notification;

public record NotificationSummaryResponse(
    IReadOnlyList<NotificationResponse> Notifications,
    int UnreadCount
);
