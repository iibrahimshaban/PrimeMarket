namespace PrimeMarket.Contracts.Brand;

public record SellerRequestDetailsResponse(
    int BrandId,
    string SellerId,
    string BrandName,
    string? Description,
    string? LogoUrl,
    string Street,
    string City,
    string Country,
    double? Latitude,
    double? Longitude,
    string ApplicantFullName,
    string ApplicantEmail,
    string? ApplicantProfilePicture
);