using ContactsAPI.DTOs;
using FluentValidation;

namespace ContactsAPI.Validators
{
    public class ContactValidator : AbstractValidator<ContactDTO>
    {
        public ContactValidator()
        {
            RuleFor(property => property.LastName)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(property => property.Nif)
                .Length(9).WithMessage("NIF length should be 9 characters.")
                .Matches(@"^[XYZ]?\d{7}[A-Z\d]$").WithMessage("Invalid NIF format.");

        }
    }
}
