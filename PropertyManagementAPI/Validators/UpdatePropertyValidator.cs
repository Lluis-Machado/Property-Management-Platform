using FluentValidation;
using PropertyManagementAPI.DTOs;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Validators
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyDTO>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }
    }
}
