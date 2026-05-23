namespace PrimeMarket.Contracts.Authentication
{
    public record MyResetPasswordRequest(
        string Email,
        string Code,
        string NewPassword
    );
}
