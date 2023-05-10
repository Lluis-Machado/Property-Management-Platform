using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class DepreciationValidator : AbstractValidator<Depreciation>
    {
        public DepreciationValidator()
        {
            RuleFor(Depreciation => Depreciation.FixedAssetId)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Depreciation => Depreciation.PeriodStart)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Depreciation => Depreciation.PeriodEnd)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Depreciation => Depreciation.PeriodStart)
                .LessThanOrEqualTo(Depreciation => Depreciation.PeriodEnd).WithMessage("{PropertyName} cannot be after PeriodEnd");

            RuleFor(Depreciation => Depreciation.PeriodEnd)
                .GreaterThanOrEqualTo(Depreciation => Depreciation.PeriodStart).WithMessage("{PropertyName} cannot be before PeriodStart");

            //RuleFor(Depreciation => Depreciation.Amount)
            //    .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            //    .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
