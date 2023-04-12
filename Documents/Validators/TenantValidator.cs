using Documents.Models;
using FluentValidation;

namespace Documents.Validators
{
    public class TenantValidator : AbstractValidator<Tenant>
    {
        public TenantValidator()
        {
            RuleFor(tenant => tenant.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Matches("^(?!-)[a-z0-9](?:[a-z0-9-]{1,61}[a-z0-9])?$(?<!-)");
        }
    }
}
