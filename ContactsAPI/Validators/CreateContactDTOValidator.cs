using ContactsAPI.DTOs;
using FluentValidation;

namespace ContactsAPI.Validators
{
    public class CreateContactDTOValidator : AbstractValidator<CreateContactDto>
    {
        public CreateContactDTOValidator()
        {
            RuleFor(property => property.LastName)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

        }
    }
}