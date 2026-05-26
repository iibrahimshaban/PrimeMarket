using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using PrimeMarket.Authentication;
using PrimeMarket.Contracts.Authentication;
using PrimeMarket.Errors;
using SurveyBasket.Contracts.Authentication;
using SurveyBasket.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace PrimeMarket.Services.Authentication
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtProvider jwtProvider,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthService> logger, IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor
        ) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtToken = jwtProvider;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private static readonly int RefreshTokenExpiryInDays = 90;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var (token, expiresIn) = _jwtToken.GenerateJwtToken(user, roles);

                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryInDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiryDate
                });

                await _userManager.UpdateAsync(user);
                return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, 
                    refreshToken, refreshTokenExpiryDate, user.ProfilePictureUrl));
            }

            var error = result.IsNotAllowed ? UserErrors.EmailNotConfirmed
                        : result.IsLockedOut ? UserErrors.LockedUser
                        : UserErrors.InvalidCredentials;

            return Result.Failure<AuthResponse>(error);
        }

        public async Task<Result> RegisterAsync(RegisterReq registerReq, CancellationToken cancellationToken)
        {
            var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == registerReq.Email, cancellationToken);

            if (emailIsExist)
                return Result.Failure(UserErrors.DuplicatedEmail);

            var user = registerReq.Adapt<ApplicationUser>();
            user.UserName = registerReq.Email;

            var result = await _userManager.CreateAsync(user, registerReq.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                _logger.LogInformation("Confirmation code: {code}", code);

                await SendConfirmationEmail(user, code);
                await _userManager.AddToRoleAsync(user, DefaultRoles.Customer);
                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(UserErrors.InvalidCode);

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = request.Code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                
            }
            catch (FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                
                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ForgetPasswordConfirmAsync(string email)
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserErrors.EmailNotConfirmed);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Reset code: {code}", code);

            await SendForgetPasswordConfirmationEmail(user, code);

            return Result.Success();
        }
        public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmail request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(MyResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);

            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
        }

        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
                }
            );

            await _emailSender.SendEmailAsync(user.Email!, "✅ Prime Market: Email Confirmation", emailBody);
        }

        private async Task SendForgetPasswordConfirmationEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/ForgetPassword?userId={user.Id}&code={code}" }
                }
            );

            await _emailSender.SendEmailAsync(user.Email!, "✅ Prime Market: ForgetPassword Confirmation", emailBody);
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string Refreshtoken, CancellationToken cancellationToken = default)
        {
            var result = _jwtToken.ValidateToken(Token);

            if (result.IsFailure)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            if (await _userManager.FindByIdAsync(result.Value) is not { } user)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == Refreshtoken && x.IsActivated);

            if (userRefreshToken == null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefresh);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var Roles = await _userManager.GetRolesAsync(user);
            (var NewToken, var ExpiryIn) = _jwtToken.GenerateJwtToken(user, Roles);

            var NewRefreshToken = GenerateRefreshToken();
            var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryInDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = NewRefreshToken,
                ExpiresOn = RefreshTokenExpiryDate,
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName
            , NewToken, ExpiryIn, NewRefreshToken, RefreshTokenExpiryDate);

            return Result.Success(response);
        }

        public async Task<Result> RevokeRefreshTokenAsync(string Token, string Refreshtoken, CancellationToken cancellationToken = default)
        {
            var result = _jwtToken.ValidateToken(Token);

            if (result.IsFailure)
                return Result.Failure(UserErrors.InvalidJwtToken);

            if (await _userManager.FindByIdAsync(result.Value) is not { } user)
                return Result.Failure(UserErrors.InvalidJwtToken);

            if (user.IsDisabled)
                return Result.Failure(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure(UserErrors.LockedUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == Refreshtoken && x.IsActivated);

            if (userRefreshToken == null)
                return Result.Failure(UserErrors.InvalidRefresh);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
