namespace PrimeMarket.Contracts.Brand;

public record BecomeSelerRequest(
    string BrandName,
    string? Description,
    IFormFile Logo,
    string Street,
    string City,
    string Country,
    double? Latitude,
    double? Longitude
);
