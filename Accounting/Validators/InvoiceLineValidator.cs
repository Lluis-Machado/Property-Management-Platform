using Accounting.Models;
using FluentValidation;

namespace Accounting.Validators
{
    public class InvoiceLineValidator : AbstractValidator<InvoiceLine>
    {
        public InvoiceLineValidator() 
        {
            RuleFor(Line => Line.LineNumber).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThanOrEqualTo(1).WithMessage("{PropertyName} cannot be less than 1");

            RuleFor(Line => Line.ArticleRefNumber).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Line => Line.ArticleName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches(@"^[\p{Ll}\s]+$").WithMessage("{PropertyName} cannot contain special characters")
                .Matches(@"^[\p{Ll}\s]{2,256}$").WithMessage("{PropertyName} has to be between 2 and 256 characters long");

            RuleFor(Line => Line.Tax).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative");

            RuleFor(Line => Line.Quantity).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThanOrEqualTo(1).WithMessage("{PropertyName} cannot be less than 1");

            RuleFor(Line => Line.UnitPrice).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .GreaterThan(0).WithMessage("{PropertyName} has to be greater than 0");

            RuleFor(Line => Line.TotalPrice).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Line => Line.DateRefFrom).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Line => Line.DateRefTo).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");

            RuleFor(Line => Line.ExpenseTypeId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");
        }
    }
}
