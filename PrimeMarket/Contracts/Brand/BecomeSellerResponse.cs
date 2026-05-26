namespace PrimeMarket.Contracts.Brand;

public record BecomeSellerResponse(
    string SellerId,
    string BrandName,
    string? Description,
    string? Logo,
    string Location
 );
