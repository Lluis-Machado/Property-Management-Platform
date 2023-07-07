using FluentValidation;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Validators
{
    public class CreateDeclarationValidator : AbstractValidator<CreateDeclarationDTO>
    {
        public CreateDeclarationValidator()
        {
            RuleFor(declarant => declarant.DeclarantId).NotNull()
                .WithMessage("{PropertyName} cannot be null");
        }
    }
}
