using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreateDepreciationDTOValidator : AbstractValidator<CreateDepreciationDTO>
    {
        public CreateDepreciationDTOValidator() { 
        }
    }
}
