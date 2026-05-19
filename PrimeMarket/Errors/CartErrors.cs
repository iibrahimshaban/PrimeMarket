namespace PrimeMarket.Errors;

public static class CartErrors
{
    public static readonly Error ProductNotFound = new(
        "Cart.ProductNotFound", "Product not found or is no longer available", StatusCodes.Status404NotFound);

    public static readonly Error ItemNotFound = new(
        "Cart.ItemNotFound", "This item is not in your cart", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyInCart = new(
        "Cart.AlreadyExists", "This product is already in your cart", StatusCodes.Status409Conflict);

    public static readonly Error InvalidQuantity = new(
        "Cart.InvalidQuantity", "Quantity must be at least 1", StatusCodes.Status400BadRequest);

    public static readonly Error InsufficientStock = new(
        "Cart.InsufficientStock", "Not enough stock available", StatusCodes.Status400BadRequest);
}
