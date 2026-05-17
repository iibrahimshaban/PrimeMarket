using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace PrimeMarket.Services;

public class CloudinaryService(Cloudinary cloudinary) : ICloudinaryService
{
    private readonly Cloudinary _cloudinary = cloudinary;

    public async Task<string> UploadImageAsync(IFormFile file, string folder, string publicId)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            PublicId = publicId,
            Overwrite = true,
            Transformation = new Transformation()
                .Width(500).Height(500).Crop("fill").Gravity("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new InvalidOperationException(result.Error.Message);

        return result.SecureUrl.ToString();
    }

    //---------------------------------------------------------------------------------------------------
    public async Task DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        await _cloudinary.DestroyAsync(deleteParams);
    }

    //---------------------------------------------------------------------------------------------------

    public async Task DeleteImageByUrlAsync(string imageUrl)
    {
        var publicId = ExtractPublicId(imageUrl);

        if (string.IsNullOrEmpty(publicId))
            return;

        await DeleteImageAsync(publicId);
    }

    //---------------------------------------------------------------------------------------------------
    private static string ExtractPublicId(string url)
    {
        try
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');

            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex < 0) return string.Empty;

            var afterUpload = segments.Skip(uploadIndex + 1).ToArray();

            if (afterUpload.Length > 0
                && afterUpload[0].StartsWith('v')
                && afterUpload[0].Length > 1
                && afterUpload[0][1..].All(char.IsDigit))
            {
                afterUpload = afterUpload.Skip(1).ToArray();
            }

            var publicIdWithExtension = string.Join("/", afterUpload);
            var dotIndex = publicIdWithExtension.LastIndexOf('.');

            return dotIndex >= 0
                ? publicIdWithExtension[..dotIndex]
                : publicIdWithExtension;
        }
        catch
        {
            return string.Empty;
        }
    }
}
