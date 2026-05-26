namespace PrimeMarket.Errors;

public static class BrandErrors
{
    public static Error BrandNotFound =>
        new Error("Brand.NotFound", "The specified brand was not found.",StatusCodes.Status404NotFound);
}
