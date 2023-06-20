using AccountingAPI.Models;
using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreatePeriodDTOValidator : AbstractValidator<CreatePeriodDTO>
    {
        public CreatePeriodDTOValidator()
        {
            RuleFor(Period => Period.Year)
                .Must(BeAValidYear)
                .WithMessage("Invalid {PropertyName}");

            RuleFor(Period => Period.Month)
                 .Must(BeAValidMonth)
                .WithMessage("Invalid {PropertyName}");
        }

        private bool BeAValidYear(int year)
        {
            return year >= 1900 && year <= DateTime.Now.Year; // Modify the range if needed
        }

        private bool BeAValidMonth(int month)
        {
            return month >= 1 && month <= 12;
        }
    }
}
