using AccountingAPI.DTOs;
using FluentValidation;

namespace AccountingAPI.Validators
{
    public class UpdateExpenseCategoryDTOValidator : AbstractValidator<UpdateExpenseCategoryDTO>
    {
        public UpdateExpenseCategoryDTOValidator()
        {
            RuleFor(Type => Type.ExpenseTypeCode)
                .IsEnumName(typeof(ExpenseType))
                .WithMessage("Invalid {PropertyName}");

            RuleFor(Type => Type.Name)
                .Length(3, 255)
                .WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");
        }

        private enum ExpenseType
        {
            UAT,
            UAV,
            BAT,
            BAV,
            Asset
        }
    }
}
