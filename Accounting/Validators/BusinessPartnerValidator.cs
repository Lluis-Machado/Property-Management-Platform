using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class BusinessPartnerValidator : AbstractValidator<BusinessPartner>
    {
        public BusinessPartnerValidator()
        {
            RuleFor(businessPartner => businessPartner.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{Ll}\s]+$").WithMessage("{PropertyName} has to be in lower case")
                .Matches(@"^[\p{Ll}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");


            RuleFor(businessPartner => businessPartner.VATNumber)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("{PropertyName} only accepts Alfanumerics values, whitespaces and '-'")
                .Matches(@"^[a-zA-Z0-9\s\-]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");


            RuleFor(businessPartner => businessPartner.AccountID)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("{PropertyName} only accepts Alfanumerics values, whitespaces and '-'")
                .Matches(@"^[a-zA-Z0-9\s\-]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");


            RuleFor(businessPartner => businessPartner.Type)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("{PropertyName} only accepts Alfanumerics values, whitespaces and '-'")
                .Matches(@"^[a-zA-Z0-9\s\-]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");
        }
    }
}
