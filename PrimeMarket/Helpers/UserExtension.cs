using System.Security.Claims;

namespace PrimeMarket.Helpers;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal User)
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
