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
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative");

            RuleFor(Line => Line.Quantity)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThanOrEqualTo(1).WithMessage("{PropertyName} cannot be less than 1");

            RuleFor(Line => Line.UnitPrice)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");
        }
    }
}
