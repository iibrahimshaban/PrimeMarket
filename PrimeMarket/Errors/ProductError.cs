namespace PrimeMarket.Errors;

public static class ProductError
{
    public static readonly Error ProductNotFound = new(
       "Product.ProductNotFound", "product not found ", StatusCodes.Status404NotFound);

    public static readonly Error UnauthorizedAction = new(
        "Product.Unauthorized", "You are not allowed to modify this product", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidCategory = new(
        "Product.InvalidCategory", "One or more categories are invalid", StatusCodes.Status400BadRequest);

    public static readonly Error ImageNotFound = new(
       "Product.ImageNotFound", "Image not found for this product", StatusCodes.Status404NotFound);

    public static readonly Error CannotDeletePrimaryImage = new(
        "Product.CannotDeletePrimaryImage", "Cannot delete the primary image. Set another image as primary first.", StatusCodes.Status400BadRequest);

    public static readonly Error CannotDeleteOnlyImage = new(
        "Product.CannotDeleteOnlyImage", "Cannot delete the only image of a product.", StatusCodes.Status400BadRequest);
    public static readonly Error InsufficientStock = new(
        "Product.InsufficientStock", "Insufficient stock for one or more products in the cart.", StatusCodes.Status400BadRequest);
}
