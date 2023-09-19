using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreatePeriodDTOValidator : AbstractValidator<CreatePeriodDTO>
    {
        public CreatePeriodDTOValidator()
        {
            RuleFor(Period => Period.Year)
                .InclusiveBetween(1900, DateTime.Now.Year)
                .WithMessage("Year must be between {From} and {To}");

            RuleFor(Period => Period.Month)
                .InclusiveBetween(1, 12)
                .WithMessage("Month must be between {From} and {To}");
        }
    }
}
