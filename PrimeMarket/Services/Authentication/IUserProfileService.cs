using CloudinaryDotNet.Actions;
using PrimeMarket.Contracts.Users;

namespace PrimeMarket.Services.Authentication
{
    public interface IUserProfileService
    {
        Task<Result<UserProfileResponse>> GetUserInfoAsync(string userId);
        Task<Result> UpdateUserInfoAsync(string userId, UpdateUserProfileRequest request);
        Task<Result> UpdateUserPasswordAsync(string userId, ChangePasswordRequest changePasswordRequest);
        Task<Result<string>> UploadUserProfileImageAsync(string userId, IFormFile image);
    }
}
