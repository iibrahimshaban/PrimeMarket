namespace PrimeMarket.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder, string publicId);
    Task DeleteImageAsync(string publicId);
}
