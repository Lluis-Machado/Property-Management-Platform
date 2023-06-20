using AccountingAPI.Models;
using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreateTenantDTOValidator : AbstractValidator<CreateTenantDTO>
    {
        public CreateTenantDTOValidator()
        {
            RuleFor(tenant => tenant.Name)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }
    }
}
