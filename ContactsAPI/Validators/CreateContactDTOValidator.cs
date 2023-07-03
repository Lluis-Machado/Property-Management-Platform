using ContactsAPI.DTOs;
using ContactsAPI.Models;
using FluentValidation;

namespace ContactsAPI.Validators
{
    public class CreateContactDTOValidator : AbstractValidator<CreateContactDTO>
    {
        public CreateContactDTOValidator()
        {
            RuleFor(property => property.LastName)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(property => property.NIE)
                .NotEmpty().WithMessage("NIE is required.")
                .Length(9).WithMessage("NIE length should be 9 characters.")
                .Matches(@"^[XYZ]\d{7}[A-Z]$").WithMessage("Invalid NIE format.");


        }
    }
}
