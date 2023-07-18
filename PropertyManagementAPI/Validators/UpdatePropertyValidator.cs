using FluentValidation;
using PropertiesAPI.Dtos;
using PropertiesAPI.Models;

namespace PropertiesAPI.Validators
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyDto>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
            
        }
        
    }
}
