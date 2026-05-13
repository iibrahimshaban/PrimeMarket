using Microsoft.AspNetCore.Identity;

namespace PrimeMarket.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public bool isDisabled { get; set; } = false;
}
