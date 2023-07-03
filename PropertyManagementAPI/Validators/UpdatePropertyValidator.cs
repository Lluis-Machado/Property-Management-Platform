using FluentValidation;
using PropertiesAPI.DTOs;
using PropertiesAPI.Models;

namespace PropertiesAPI.Validators
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyDTO>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(property => property.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(3, 255).WithMessage("{PropertyName} must be from {MinLength} to {MaxLength} characters long");

            RuleFor(property => property.Ownerships)
                .Must(ValidateOwnerships)
                .WithMessage("Ownership validation failed.");
        }

        private bool ValidateOwnerships(List<PropertyOwnershipDto> ownerships)
        {
            if (ownerships == null || ownerships.Count == 0)
                return false;

            decimal totalShares = ownerships.Sum(o => o.Share);
            int mainContactCount = ownerships.Count(o => o.MainOwnership);

            return totalShares == 100 && mainContactCount == 1;
        }
    }
}
