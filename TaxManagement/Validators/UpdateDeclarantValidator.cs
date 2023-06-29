using FluentValidation;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Validators
{
    public class UpdateDeclarantValidator : AbstractValidator<UpdateDeclarantDTO>
    {
        public UpdateDeclarantValidator()
        {
            RuleFor(declarant => declarant.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

        }
    }
}
