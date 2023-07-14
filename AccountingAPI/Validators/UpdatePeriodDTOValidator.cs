using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdatePeriodDTOValidator : AbstractValidator<UpdatePeriodDTO>
    {
        public UpdatePeriodDTOValidator()
        {
        }
    }
}
