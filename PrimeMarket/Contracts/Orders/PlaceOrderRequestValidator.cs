namespace PrimeMarket.Contracts.Orders;

public class PlaceOrderRequestValidator : AbstractValidator<PlaceOrderRequest>
{
    public PlaceOrderRequestValidator()
    {
        RuleFor(x => x.AddressId)
            .GreaterThan(0).WithMessage("A valid address must be selected.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method.");

        RuleFor(x => x.PromoCode)
            .MaximumLength(50).WithMessage("Promo code cannot exceed 50 characters.")
            .When(x => x.PromoCode is not null);
    }
}
