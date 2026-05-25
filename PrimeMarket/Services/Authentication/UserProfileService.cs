using Microsoft.AspNetCore.Identity;
using PrimeMarket.Contracts.Users;

namespace PrimeMarket.Services.Authentication
{
    public class UserProfileService(UserManager<ApplicationUser> userManager, ICloudinaryService cloudinaryService) : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

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

        public async Task<Result<string>> UploadUserProfileImageAsync(string userId, IFormFile image)
        {
            // Validate file
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

            if (image.Length == 0)
                return Result.Failure<string>(new Error("EmptyFile", "Image file cannot be empty", StatusCodes.Status400BadRequest));

            if (image.Length > maxFileSize)
                return Result.Failure<string>(new Error("FileTooLarge", "Image file cannot exceed 5 MB", StatusCodes.Status400BadRequest));

            if (!allowedMimeTypes.Contains(image.ContentType))
                return Result.Failure<string>(new Error("InvalidFileType", "Only JPEG, PNG, GIF, and WebP images are allowed", StatusCodes.Status400BadRequest));

            try
            {
                // Get current user to retrieve old image URL
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Failure<string>(new Error("UserNotFound", "User not found", StatusCodes.Status404NotFound));

                // Delete old profile picture if it exists
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    await _cloudinaryService.DeleteImageByUrlAsync(user.ProfilePictureUrl);
                }

                // Upload new image to Cloudinary
                var imageUrl = await _cloudinaryService.UploadImageAsync(image, "user_profiles", userId);

                // Update user's profile picture URL
                user.ProfilePictureUrl = imageUrl;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    var error = updateResult.Errors.First();
                    return Result.Failure<string>(new Error(error.Code, error.Description, StatusCodes.Status500InternalServerError));
                }

                return Result.Success(imageUrl);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new Error("UploadError", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
    }
}
