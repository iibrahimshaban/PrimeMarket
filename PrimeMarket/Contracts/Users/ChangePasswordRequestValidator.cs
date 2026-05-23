namespace PrimeMarket.Contracts.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password cannot be the same as current password");
        }
    }
}
