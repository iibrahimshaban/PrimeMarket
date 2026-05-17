namespace PrimeMarket.Contracts.Authentication
{
    public record LogInRequest(
        string Email,
        string Password
    );
}
