using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreateLoanDTOValidator : AbstractValidator<CreateLoanDTO>
    {
        public CreateLoanDTOValidator()
        {
            RuleFor(loan => loan.Concept)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{L}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{L}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");

            RuleFor(loan => loan.Amount)
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
