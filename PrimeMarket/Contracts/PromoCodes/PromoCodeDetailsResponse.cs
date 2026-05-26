namespace PrimeMarket.Contracts.PromoCodes;

public record PromoCodeDetailsResponse(
    int Id,
    string Code,
    string DiscountType,
    decimal DiscountValue,
    int UsageLimit,
    int UsedCount,
    DateTime ExpiresAt,
    bool IsActive,
    string CreatedBy,
    DateTime CreatedOn,
    ICollection<PromoCodeOrderResponse> Orders
);
