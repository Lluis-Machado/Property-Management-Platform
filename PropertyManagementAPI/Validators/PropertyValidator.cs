using FluentValidation;
using PropertiesAPI.DTOs;
using PropertiesAPI.Models;

namespace PropertiesAPI.Validators
{
    public class PropertyValidator : AbstractValidator<PropertyDto>
    {
        public PropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
            
        }
    }
}
