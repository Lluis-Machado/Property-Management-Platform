using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class ExpenseTypeValidator : AbstractValidator<ExpenseType>
    {
        public ExpenseTypeValidator()
        {
            RuleFor(Type => Type.Code).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Type => Type.Description).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{L}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{L}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");
        }
    }
}
