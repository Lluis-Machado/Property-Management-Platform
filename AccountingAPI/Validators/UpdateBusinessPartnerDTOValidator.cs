using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdateBusinessPartnerDTOValidator : AbstractValidator<UpdateBusinessPartnerDTO>
    {
        public UpdateBusinessPartnerDTOValidator()
        {
            RuleFor(businessPartner => businessPartner.Name)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");


            RuleFor(businessPartner => businessPartner.VATNumber)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }
    }
}
