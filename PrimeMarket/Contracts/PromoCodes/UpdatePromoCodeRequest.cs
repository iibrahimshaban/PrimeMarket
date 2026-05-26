namespace PrimeMarket.Contracts.PromoCodes;

public record UpdatePromoCodeRequest(
    string? Code,
    DiscountType? DiscountType,
    decimal? DiscountValue,
    int? UsageLimit,
    DateTime? ExpiresAt,
    bool? IsActive
);
