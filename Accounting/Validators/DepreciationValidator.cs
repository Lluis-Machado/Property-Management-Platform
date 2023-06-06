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

            RuleFor(Depreciation => Depreciation.PeriodId)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

        }
    }
}
