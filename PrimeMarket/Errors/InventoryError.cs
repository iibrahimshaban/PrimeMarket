namespace PrimeMarket.Errors
{
    public static class InventoryError
    {
        public static readonly Error ProductNotFound = new(
            "Inventory.ProductNotFound", "Product not found", StatusCodes.Status404NotFound);

        public static readonly Error UnauthorizedAction = new(
            "Inventory.Unauthorized", "You are not allowed to modify this product's inventory", StatusCodes.Status403Forbidden);

        public static readonly Error InsufficientStock = new(
            "Inventory.InsufficientStock", "Insufficient stock to apply this adjustment", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidQuantity = new(
            "Inventory.InvalidQuantity", "Quantity change cannot be zero", StatusCodes.Status400BadRequest);
    }
}
