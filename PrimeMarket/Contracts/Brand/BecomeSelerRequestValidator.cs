namespace PrimeMarket.Contracts.Brand;

public class BecomeSelerRequestValidator : AbstractValidator<BecomeSelerRequest>
{
    public BecomeSelerRequestValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.Logo)
            .NotNull()
            .Must(file => file.Length > 0).WithMessage("Logo file cannot be empty.")
            .Must(file => file.ContentType.StartsWith("image/")).WithMessage("Logo must be an image.");

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);
    }
}
