using FluentValidation;
using PropertyManagementAPI.DTOs;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Validators
{
    public class PropertyValidator : AbstractValidator<PropertyDTO>
    {
        public PropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(property => property.Deleted)
               .Equal(false).When(property => property.Id.Equals(Guid.Empty)).WithMessage("{PropertyName} must be false");
        }
    }
}
