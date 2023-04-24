using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class AmortizationConfigValidator : AbstractValidator<AmortizationConfig>
    {
        public AmortizationConfigValidator() 
        {
            RuleFor(config => config.Type).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");


            RuleFor(config => config.AmortizationPercent).Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be less than 0");
        }
    }
}
