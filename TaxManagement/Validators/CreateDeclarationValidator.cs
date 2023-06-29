using FluentValidation;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Validators
{
    public class CreateDeclarationValidator : AbstractValidator<CreateDeclarationDTO>
    {
        public CreateDeclarationValidator()
        {
            RuleFor(declarant => declarant.Deleted)
               .Equal(false).When(declarant => declarant.Id.Equals(Guid.Empty)).WithMessage("{PropertyName} must be false");
        }
    }
}
