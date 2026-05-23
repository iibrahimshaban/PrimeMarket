using Microsoft.AspNetCore.Identity;
using PrimeMarket.Contracts.Users;

namespace PrimeMarket.Services.Authentication
{
    public class UserProfileService(UserManager<ApplicationUser> userManager) : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Result<UserProfileResponse>> GetUserInfoAsync(string userId)
        {
            var user = await _userManager.Users.
                                Where(x => x.Id == userId)
                                .ProjectToType<UserProfileResponse>()
                                .SingleAsync();
            return Result.Success(user);
        }

        public async Task<Result> UpdateUserInfoAsync(string userId, UpdateUserProfileRequest request)
        {
            await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.FirstName, request.FirstName)
                    .SetProperty(x => x.LastName, request.LastName)
            );

            return Result.Success();
        }

        public async Task<Result> UpdateUserPasswordAsync(string userId, ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!,
                changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    }
}
