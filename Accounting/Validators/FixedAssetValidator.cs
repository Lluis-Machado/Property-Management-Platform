using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class FixedAssetValidator : AbstractValidator<FixedAsset>
    {
        public FixedAssetValidator()
        {
            RuleFor(Asset => Asset.InvoiceId)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");


            RuleFor(Asset => Asset.Description)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{L}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{L}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");


            RuleFor(Asset => Asset.CapitalizationDate)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");


            RuleFor(Asset => Asset.AcquisitionAndProductionCosts)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
