using Microsoft.AspNetCore.Identity;
using PrimeMarket.Authentication;
using PrimeMarket.Contracts.Authentication;
using PrimeMarket.Contracts.Products;
using PrimeMarket.Errors;

namespace PrimeMarket.Services.Authentication
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtToken = jwtProvider;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            var isValidUser = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidUser)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            return Result.Success(GenerateJwtTokenHelper(user));
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterReq registerReq, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == registerReq.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

            var user = registerReq.Adapt<ApplicationUser>();
            user.UserName = registerReq.Email;

            var result = await _userManager.CreateAsync(user, registerReq.Password);

            if (result.Succeeded)
            {
                return Result.Success(GenerateJwtTokenHelper(user));
            }

            var error = result.Errors.First();

            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        private AuthResponse GenerateJwtTokenHelper(ApplicationUser user)
        {
            var (token, expiresIn) = _jwtToken.GenerateJwtToken(user);
            return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn);
        }
    }
}
