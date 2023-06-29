using FluentValidation;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Validators
{
    public class UpdateDeclarationValidator : AbstractValidator<UpdateDeclarationDTO>
    {
        public UpdateDeclarationValidator()
        {
            RuleFor(declarant => declarant.Deleted)
               .Equal(false).When(declarant => declarant.Id.Equals(Guid.Empty)).WithMessage("{PropertyName} must be false");
        }
    }
}
