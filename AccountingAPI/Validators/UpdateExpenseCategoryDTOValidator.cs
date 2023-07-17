using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdateExpenseCategoryDTOValidator : AbstractValidator<UpdateExpenseCategoryDTO>
    {
        public UpdateExpenseCategoryDTOValidator()
        {
            RuleFor(Type => Type.ExpenseTypeCode)
                .IsEnumName(typeof(Utilities.ExpenseTypeCodes.ExpenseType))
                .WithMessage("Invalid {PropertyName}");

            RuleFor(Type => Type.Name)
                .Length(3, 255)
                .WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }
    }
}
