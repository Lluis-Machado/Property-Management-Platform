using FluentValidation;
using TaxManagement.Models;

namespace TaxManagement.Validators
{
    public class DeclarationValidator : AbstractValidator<Declaration>
    {
        public DeclarationValidator()
        {
            RuleFor(declarant => declarant.Deleted)
               .Equal(false).When(declarant => declarant.Id.Equals(Guid.Empty)).WithMessage("{PropertyName} must be false");
        }
    }
}
