namespace PrimeMarket.Contracts.PromoCodes;

public record PromoCodeValidationResponse(
    bool IsValid,
    decimal DiscountAmount,
    string? ErrorMessage
);
