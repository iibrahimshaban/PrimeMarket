namespace PrimeMarket.Errors;

public static class AddressError
{
    public static readonly Error NotFound =
        new("Address.NotFound", "Address not found.", StatusCodes.Status404NotFound);
}