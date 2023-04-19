using FluentValidation;
using Accounting.Models;

namespace Accounting.Validators
{
    public class AccountValidator : AbstractValidator<Tenant>
    {
        public AccountValidator()
        {
            RuleFor(account => account.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{L}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{L}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");
        }
    }
}
