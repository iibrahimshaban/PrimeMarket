using Microsoft.AspNetCore.SignalR;
using PrimeMarket.Contracts.Notification;
using PrimeMarket.Hubs;

namespace PrimeMarket.Services;

public class NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hub) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IHubContext<NotificationHub> _hub = hub;

    public async Task SendToUserAsync(string userId, string title, string message, string type, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(ct);

        await _hub.Clients
            .Group($"user_{userId}")
            .SendAsync("ReceiveNotification", MapToResponse(notification), ct);
    }

    public async Task BroadcastAsync(string title, string message, string type, CancellationToken ct = default)
    {
        var userIds = await _context.Users.Select(u => u.Id).ToListAsync(ct);

        var notifications = userIds.Select(uid => new Notification
        {
            UserId = uid,
            Title = title,
            Message = message,
            Type = type
        }).ToList();

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync(ct);

        await _hub.Clients.All.SendAsync("ReceiveNotification", new { Title = title, Message = message, Type = type }, ct);
    }

    public async Task<NotificationSummaryResponse> GetUserNotificationsAsync(string userId, CancellationToken ct = default)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => MapToResponse(n))
            .ToListAsync(ct);

        var unreadCount = notifications.Count(n => !n.IsRead);

        return new NotificationSummaryResponse(notifications, unreadCount);
    }

    public async Task<Result<NotificationResponse>> GetByIdAsync(int id, string userId, CancellationToken ct = default)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, ct);

        if (notification is null)
            return Result.Failure<NotificationResponse>(NotificationErrors.NotFound);

        return Result.Success(MapToResponse(notification));
    }

    public async Task<Result> MarkAsReadAsync(int id, string userId, CancellationToken ct = default)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, ct);

        if (notification is null)
            return Result.Failure(NotificationErrors.NotFound);

        if (notification.IsRead)
            return Result.Success();

        notification.IsRead = true;
        notification.ReadOn = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> MarkAllAsReadAsync(string userId, CancellationToken ct = default)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadOn, DateTime.UtcNow), ct);

        return Result.Success();
    }

    private static NotificationResponse MapToResponse(Notification n) =>
        new(n.Id, n.Title, n.Message, n.Type, n.IsRead, n.ReadOn, n.CreatedAt);
}
