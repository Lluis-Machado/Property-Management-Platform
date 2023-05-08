using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class DepreciationValidator : AbstractValidator<Depreciation>
    {
        public DepreciationValidator()
        {
            RuleFor(Depreciation => Depreciation.FixedAssetId)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty");

            RuleFor(Depreciation => Depreciation.PeriodStart)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty");

            RuleFor(Depreciation => Depreciation.PeriodEnd)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty");

            RuleFor(Depreciation => Depreciation.PeriodStart)
                .LessThanOrEqualTo(Depreciation => Depreciation.PeriodEnd).WithMessage("{Propertyname} cannot be after PeriodEnd");

            RuleFor(Depreciation => Depreciation.PeriodEnd)
                .GreaterThanOrEqualTo(Depreciation => Depreciation.PeriodStart).WithMessage("{Propertyname} cannot be before PeriodStart");

            RuleFor(Depreciation => Depreciation.Amount)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
