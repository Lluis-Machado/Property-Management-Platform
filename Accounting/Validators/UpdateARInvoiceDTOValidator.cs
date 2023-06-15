using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdateARInvoiceDTOValidator : AbstractValidator<UpdateARInvoiceDTO>
    {
        public UpdateARInvoiceDTOValidator()
        {

            RuleFor(Invoice => Invoice.RefNumber)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Invoice => Invoice.Date)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");
        }
    }
}
