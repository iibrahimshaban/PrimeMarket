using Microsoft.AspNetCore.Identity;
using PrimeMarket.Authentication;
using PrimeMarket.Contracts.Authentication;

namespace PrimeMarket.Services.Authentication
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtToken = jwtProvider;

        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            var isValidUser = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidUser)
                return null;

            var (token, expiresIn) = _jwtToken.GenerateJwtToken(user);

            return new AuthResponse(user.Id , user.Email, user.FirstName, user.LastName, token, expiresIn);
        }
    }
}
