using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Users;
using PrimeMarket.Helpers;
using PrimeMarket.Persistence.Migrations;
using PrimeMarket.Services.Authentication;

namespace PrimeMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(IUserProfileService userProfile) : ControllerBase
    {
        private readonly IUserProfileService _userProfile = userProfile;

        [HttpGet("Info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _userProfile.GetUserInfoAsync(User.GetUserId()!);

            return Ok(result.Value);
        }
        [HttpPost("Info")]
        public async Task<IActionResult> GetUserInfo([FromBody] UpdateUserProfileRequest request)
        {
            await _userProfile.UpdateUserInfoAsync(User.GetUserId()!, request);
            return NoContent();
        }
        [HttpPut("Change-Password")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] ChangePasswordRequest updateApplicationUserPassword)
        {
            var result = await _userProfile.UpdateUserPasswordAsync(User.GetUserId()!, updateApplicationUserPassword);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPost("Profile-Image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] UploadUserProfileImageRequest request)
        {
            var result = await _userProfile.UploadUserProfileImageAsync(User.GetUserId()!, request.Image);

            return result.IsSuccess ? Ok(new { imageUrl = result.Value }) : result.ToProblem();
        }
    }
}
