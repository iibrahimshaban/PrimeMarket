namespace PrimeMarket.Contracts.Users
{
    public record UpdateUserProfileRequest(
        string FirstName,
        string LastName
    );
}
