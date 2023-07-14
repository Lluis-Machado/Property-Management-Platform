using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreateBusinessPartnerDTOValidator : AbstractValidator<CreateBusinessPartnerDTO>
    {
        public CreateBusinessPartnerDTOValidator()
        {
            RuleFor(businessPartner => businessPartner.Name)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");


            RuleFor(businessPartner => businessPartner.VATNumber)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }
    }
}
