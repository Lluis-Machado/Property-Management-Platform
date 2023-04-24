using FluentValidation;
using Accounting.Models;

namespace Accounting.Validators
{
    public class LoanValidator : AbstractValidator<Loan>
    {
        public LoanValidator()
        {
            RuleFor(loan => loan.BusinessPartnerID).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(loan => loan.Concept).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{L}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{L}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");

            RuleFor(loan => loan.Amount)
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");

            RuleFor(loan => loan.AmountPaid)
                .LessThanOrEqualTo(loan => loan.Amount).WithMessage("{PropertyName} has to be lower than {ComparisonProperty}");
        }
    }
}
