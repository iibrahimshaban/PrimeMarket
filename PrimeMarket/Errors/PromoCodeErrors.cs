namespace PrimeMarket.Errors;

public static class PromoCodeErrors
{
    public static readonly Error CodeNotFound =
        new("PromoCode.NotFound", "Promo code not found.", StatusCodes.Status404NotFound);
    public static readonly Error CodeAlreadyExsist =
        new("PromoCode.CodeAlreadyExsist", "Promo code already exists.", StatusCodes.Status409Conflict);


}
