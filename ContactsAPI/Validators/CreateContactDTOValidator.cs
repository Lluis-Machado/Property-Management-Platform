using FluentValidation;
using ContactsAPI.Models;

namespace ContactsAPI.Validators
{
    public class CreateContactDTOValidator : AbstractValidator<CreateContactDTO>
    {
        public CreateContactDTOValidator()
        {
            RuleFor(property => property.LastName)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

        }
    }
}
