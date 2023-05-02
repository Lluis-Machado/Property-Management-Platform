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

            RuleFor(Depreciation => Depreciation.Period)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty");

            RuleFor(Depreciation => Depreciation.Amount)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
