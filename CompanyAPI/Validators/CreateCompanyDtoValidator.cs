using CompanyAPI.Dtos;
using FluentValidation;

namespace CompanyAPI.Validators
{
    public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
    {
        public CreateCompanyDtoValidator()
        {
            RuleFor(company => company.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(company => company.Nif)
                .Length(9).WithMessage("NIF length should be 9 characters.")
                .Matches(@"^[XYZ]?\d{7}[A-Z\d]$").WithMessage("Invalid NIF format.");
        }
    }
}
