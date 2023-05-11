using FluentValidation;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Validators
{
    public class PropertyValidator : AbstractValidator<Property>
    {
        public PropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(property => property.Deleted)
               .Equal(false).When(property => property._id.Equals(Guid.Empty)).WithMessage("{PropertyName} must be false");
        }
    }
}
