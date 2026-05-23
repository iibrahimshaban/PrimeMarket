namespace PrimeMarket.Errors;

public static class OrderError
{
    public static readonly Error CartEmpty =
        new("Order.CartEmpty", "Your cart is empty.",StatusCodes.Status404NotFound);

    public static readonly Error AddressNotFound =
        new("Order.AddressNotFound", "Address not found.",StatusCodes.Status404NotFound);

    public static readonly Error InvalidPromoCode =
        new("Order.InvalidPromoCode", "Promo code is invalid or expired.",StatusCodes.Status400BadRequest);

    public static readonly Error NotFound = 
        new("Order.NotFound", "Order not found.", StatusCodes.Status404NotFound);
}
