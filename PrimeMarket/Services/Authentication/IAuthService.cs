using PrimeMarket.Contracts.Authentication;

namespace PrimeMarket.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> RegisterAsync(RegisterReq registerReq, CancellationToken cancellationToken);
    }
}
