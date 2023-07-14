using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdateTenantDTOValidator : AbstractValidator<UpdateTenantDTO>
    {
        public UpdateTenantDTOValidator()
        {
            RuleFor(tenant => tenant.Name)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }
    }
}
