namespace PrimeMarket.Authentication
{
    public interface IJwtProvider
    {
        (string Token, int expiresIn) GenerateJwtToken(ApplicationUser user);
    }
}
