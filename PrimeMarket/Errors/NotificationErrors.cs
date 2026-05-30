namespace PrimeMarket.Errors;

public static class NotificationErrors
{
    public static readonly Error NotFound = new("Notification.NotFound", "Notification not found.",StatusCodes.Status404NotFound);
}