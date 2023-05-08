using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
    {
        public InvoiceLineValidator()
        {
            RuleFor(Line => Line.ArticleRefNumber)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Line => Line.ArticleName)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{Ll}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{Ll}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");

            RuleFor(Line => Line.Tax)
                .NotNull().WithMessage("{PropertyName} cannot be null")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative");

            RuleFor(Line => Line.Quantity)
                .NotNull().WithMessage("{PropertyName} cannot be null");
                //.GreaterThanOrEqualTo(1).WithMessage("{PropertyName} cannot be less than 1");  // Allow quantities to be negative for discounts

            RuleFor(Line => Line.UnitPrice)
                .NotNull().WithMessage("{PropertyName} cannot be null")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative");
        }
    }
}
