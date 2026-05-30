using PrimeMarket.Contracts.Notification;

namespace PrimeMarket.Services;

public interface INotificationService
{
    Task SendToUserAsync(string userId, string title, string message, string type, CancellationToken ct = default);
    Task BroadcastAsync(string title, string message, string type, CancellationToken ct = default);
    Task<NotificationSummaryResponse> GetUserNotificationsAsync(string userId, CancellationToken ct = default);
    Task<Result<NotificationResponse>> GetByIdAsync(int id, string userId, CancellationToken ct = default);
    Task<Result> MarkAsReadAsync(int id, string userId, CancellationToken ct = default);
    Task<Result> MarkAllAsReadAsync(string userId, CancellationToken ct = default);
}
