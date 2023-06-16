using FluentValidation;
using TaxManagement.Models;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Validators
{
    public class CreateDeclarantValidator : AbstractValidator<CreateDeclarantDTO>
    {
        public CreateDeclarantValidator()
        {
            RuleFor(declarant => declarant.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

        }
    }
}
