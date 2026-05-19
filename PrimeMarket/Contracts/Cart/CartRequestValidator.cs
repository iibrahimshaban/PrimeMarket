

namespace PrimeMarket.Contracts.Cart;

public class CartRequestValidator : AbstractValidator<CartRequest>
{
    public CartRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
    }
}
