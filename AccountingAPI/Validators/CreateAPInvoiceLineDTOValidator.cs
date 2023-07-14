using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreateAPInvoiceLineDTOValidator : AbstractValidator<CreateAPInvoiceLineDTO>
    {
        public CreateAPInvoiceLineDTOValidator()
        {
            RuleFor(Line => Line.Description)
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(Line => Line.Tax)
                .NotNull().WithMessage("{PropertyName} cannot be null")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative");

            RuleFor(Line => Line.Quantity)
                .NotNull().WithMessage("{PropertyName} cannot be null");

            RuleFor(Line => Line.UnitPrice)
                .NotNull().WithMessage("{PropertyName} cannot be null")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative");
        }
    }
}
