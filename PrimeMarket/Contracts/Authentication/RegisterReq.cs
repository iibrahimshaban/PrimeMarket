namespace PrimeMarket.Contracts.Authentication
{
    public record RegisterReq(
        string Email,
        string Password,
        string FirstName,
        string LastName
    );
}
