using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class DepreciationConfigValidator : AbstractValidator<DepreciationConfig>
    {
        public DepreciationConfigValidator()
        {
            RuleFor(config => config.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");
            RuleFor(config => config.Type)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");


            RuleFor(config => config.DepreciationPercent)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be less than 0");

            RuleFor(config => config.MaxYears)
                .LessThanOrEqualTo(255).WithMessage("{PropertyName} cannot be more than 255");
        }
    }
}
