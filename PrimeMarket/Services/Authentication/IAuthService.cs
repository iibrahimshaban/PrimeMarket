using PrimeMarket.Contracts.Authentication;
using SurveyBasket.Contracts.Authentication;

namespace PrimeMarket.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(RegisterReq registerReq, CancellationToken cancellationToken);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmail request);
        Task<Result> ForgetPasswordConfirmAsync(string email);
        Task<Result> ResetPasswordAsync(MyResetPasswordRequest request);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string Refreshtoken,
            CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> LoginWithGoogleAsync(GoogleCredential googlecredential);
        Task<Result> RevokeRefreshTokenAsync(string Token, string Refreshtoken,
            CancellationToken cancellationToken = default);
    }
}
