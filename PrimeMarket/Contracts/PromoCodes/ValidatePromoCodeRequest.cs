namespace PrimeMarket.Contracts.PromoCodes;

public record ValidatePromoCodeRequest(
    string Code, 
    decimal CartTotal
    );
