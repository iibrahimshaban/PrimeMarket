namespace PrimeMarket.Contracts.PromoCodes;

public record CreatePromoCodeRequest(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    int UsageLimit,
    DateTime ExpiresAt
);
