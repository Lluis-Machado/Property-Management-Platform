using ContactsAPI.Models;
using FluentValidation;

namespace ContactsAPI.Validators
{
    public class UpdateContactDTOValidator : AbstractValidator<UpdateContactDTO>
    {
        public UpdateContactDTOValidator()
        {
            RuleFor(property => property.LastName)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

        }
    }
}
