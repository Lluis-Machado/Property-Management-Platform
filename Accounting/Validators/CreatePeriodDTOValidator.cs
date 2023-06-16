using AccountingAPI.Models;
using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class CreatePeriodDTOValidator : AbstractValidator<CreatePeriodDTO>
    {
        public CreatePeriodDTOValidator()
        {
        }
    }
}
