using FluentValidation;
using TaxManagementAPI.DTOs;

namespace TaxManagement.Validators
{
    // Right now there are no validations to perform on Declaration update DTO objects.
    // This class can be removed in the future
    // - Izar
    public class UpdateDeclarationValidator : AbstractValidator<UpdateDeclarationDTO>
    {
        public UpdateDeclarationValidator()
        {
            
        }
    }
}
