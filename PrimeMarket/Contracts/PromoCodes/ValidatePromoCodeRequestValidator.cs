namespace PrimeMarket.Contracts.PromoCodes;

public class ValidatePromoCodeRequestValidator : AbstractValidator<ValidatePromoCodeRequest>
{
    public ValidatePromoCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Promo code is required.")
            .MaximumLength(50).WithMessage("Promo code cannot exceed 50 characters.");

        RuleFor(x => x.CartTotal)
            .GreaterThan(0).WithMessage("Cart total must be greater than zero.");
    }
}
