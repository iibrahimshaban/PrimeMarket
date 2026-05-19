namespace PrimeMarket.Errors;

public static class WishListErrors
{
    public static readonly Error WishNotFound = new(
         "Wishlist.WishlistNotFound", "No items found in wishlist", StatusCodes.Status404NotFound);

    public static readonly Error ProductNotFound = new(
        "Wishlist.ProductNotFound", "Product not found or is no longer available", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyInWishlist = new(
        "Wishlist.AlreadyExists", "This product is already in your wishlist", StatusCodes.Status409Conflict);

    public static readonly Error ItemNotInWishlist = new(
        "Wishlist.ItemNotFound", "This product is not in your wishlist", StatusCodes.Status404NotFound);

}
