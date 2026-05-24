namespace PrimeMarket.Contracts.Authentication
{
    public record MyResetPasswordRequest(
        string UserId,
        string Code,
        string NewPassword
    );
}
