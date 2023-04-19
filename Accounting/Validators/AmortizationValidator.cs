using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class AmortizationValidator :  AbstractValidator<Amortization>
    {
        public AmortizationValidator() 
        {
            RuleFor(Amortization => Amortization.FixedAssetId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty");

            RuleFor(Amortization =>  Amortization.Period).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty");

            RuleFor(Amortization => Amortization.Amount).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{Propertyname} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
