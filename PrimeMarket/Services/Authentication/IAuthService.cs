using PrimeMarket.Contracts.Authentication;

namespace PrimeMarket.Services.Authentication
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
