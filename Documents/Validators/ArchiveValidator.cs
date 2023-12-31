﻿using DocumentsAPI.Models;
using FluentValidation;

namespace DocumentsAPI.Validators
{
    public class ArchiveValidator : AbstractValidator<Archive>
    {
        public ArchiveValidator()
        {
            RuleFor(archive => archive.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty");
            //.Matches(@"^[a-z0-9]+(-[a-z0-9]+)*$").WithMessage("Invalid {PropertyName}");
        }
    }
}
