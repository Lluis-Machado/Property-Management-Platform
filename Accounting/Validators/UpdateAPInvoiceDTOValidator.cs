using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdateAPInvoiceDTOValidator : AbstractValidator<UpdateAPInvoiceDTO>
    {
        public UpdateAPInvoiceDTOValidator()
        {

            RuleFor(Invoice => Invoice.RefNumber)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Invoice => Invoice.Date)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");
        }
    }
}
