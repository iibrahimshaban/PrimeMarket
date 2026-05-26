namespace PrimeMarket.Contracts.PromoCodes;

public record PromoCodeResponse(
    int Id,
    string Code,
    string DiscountType,
    decimal DiscountValue,
    int UsageLimit,
    int UsedCount,
    DateTime ExpiresAt,
    bool IsActive,
    string CreatedBy,
    DateTime CreatedOn
);
