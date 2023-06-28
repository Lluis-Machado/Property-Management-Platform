using AccountingAPI.DTOs;
using AccountingAPI.Utilities;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreatePeriodDTOValidator : AbstractValidator<CreatePeriodDTO>
    {
        public CreatePeriodDTOValidator()
        {
            RuleFor(Period => Period.Year)
                .Must(ValidationHelpers.IsAValidYear)
                .WithMessage("Invalid {PropertyName}");

            RuleFor(Period => Period.Month)
                 .Must(ValidationHelpers.IsAValidMonth)
                .WithMessage("Invalid {PropertyName}");
        }
    }
}
