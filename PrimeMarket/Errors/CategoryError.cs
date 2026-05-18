namespace PrimeMarket.Errors;

public static class CategoryError
{
    public static readonly Error CategoryNotFound = new(
        "Category.NotFound","Category not found", StatusCodes.Status404NotFound);

    public static readonly Error CategoryAlreadyExists = new(
        "Category.AlreadyExists","Category already exists", StatusCodes.Status409Conflict);

    public static readonly Error CategoryInUse = new(
        "Category.InUse", "Cannot delete category because it is assigned to one or more products", StatusCodes.Status400BadRequest);
}