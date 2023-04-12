using FluentValidation;
using TaxManagement.Models;

namespace TaxManagement.Validators
{
    public class DeclarantValidator : AbstractValidator<Declarant>
    {
        public DeclarantValidator()
        {
            RuleFor(declarant => declarant.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(declarant => declarant.Deleted)
               .Equal(false).When(declarant => declarant.Id.Equals(Guid.Empty)).WithMessage("{PropertyName} must be false");
        }
    }
}
