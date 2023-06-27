﻿using AccountingAPI.DTOs;
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

            RuleFor(Invoice => Invoice.Currency)
              .NotEmpty().WithMessage("{PropertyName} is required.")
              .Must(BeValidCurrencyCode).WithMessage("Invalid {PropertyName} code.");

            RuleFor(Invoice => Invoice.InvoiceLines)
                .Must(list => list is not null && list.Count > 0)
                .WithMessage("{PropertyName} cannot be empty.");
        }
        private bool BeValidCurrencyCode(string? currencyCode)
        {
            if (currencyCode is null) return false;
            return Enum.IsDefined(typeof(CurrencyCode), (string)currencyCode);
        }

        enum CurrencyCode
        {
            USD, // United States Dollar
            EUR, // Euro
        }

    }
}
