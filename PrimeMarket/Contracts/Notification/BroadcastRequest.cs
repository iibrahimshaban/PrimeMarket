namespace PrimeMarket.Contracts.Notification;


public record BroadcastRequest(
    string Title,
    string Message,
    string Type);
