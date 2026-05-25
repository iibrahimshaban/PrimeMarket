namespace PrimeMarket.Contracts.Users
{
    public class UploadUserProfileImageRequest
    {
        public IFormFile Image { get; set; } = null!;
    }
}
