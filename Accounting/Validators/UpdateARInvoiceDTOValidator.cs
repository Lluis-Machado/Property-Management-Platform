﻿using AccountingAPI.DTOs;
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

            RuleFor(Invoice => Invoice.Currency)
              .NotEmpty().WithMessage("{PropertyName} is required.")
              .Must(BeValidCurrencyCode).WithMessage("Invalid {PropertyName} code.");

            RuleFor(Invoice => Invoice.InvoiceLines)
                .Must(list => list != null && list.Count > 0)
                .WithMessage("{PropertyName} cannot be empty.");
        }

        private bool BeValidCurrencyCode(string currencyCode)
        {
            return Enum.IsDefined(typeof(CurrencyCode), currencyCode);
        }

        enum CurrencyCode
        {
            USD, // United States Dollar
            EUR, // Euro
        }
    }
}