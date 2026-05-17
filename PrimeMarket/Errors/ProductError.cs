namespace PrimeMarket.Errors;

public static class ProductError
{
    public static readonly Error ProductNotFound = new(
       "Product.ProductNotFound", "product not found ", StatusCodes.Status404NotFound);
}
