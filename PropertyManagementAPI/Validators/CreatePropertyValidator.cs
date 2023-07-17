using FluentValidation;
using PropertiesAPI.Dtos;

namespace PropertiesAPI.Validators
{
    public class CreatePropertyValidator : AbstractValidator<CreatePropertyDto>
    {
        public CreatePropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");


        }
    }
}
