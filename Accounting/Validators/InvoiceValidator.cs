using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {
            RuleFor(Invoice => Invoice.BusinessPartnerId)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Invoice => Invoice.RefNumber)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Invoice => Invoice.Date)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Invoice => Invoice.Currency)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"[A-Z]+").WithMessage("{PropertyName} has to be in uppercase");

            RuleFor(Invoice => Invoice.GrossAmount)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Invoice => Invoice.NetAmount)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .LessThan(x => x.GrossAmount);
        }
    }
}
