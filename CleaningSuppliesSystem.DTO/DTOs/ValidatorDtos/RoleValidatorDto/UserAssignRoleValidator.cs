using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RoleValidatorDto
{
    public class UserAssignRoleValidator : AbstractValidator<UserAssignRoleDto>
    {
        public UserAssignRoleValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID geçersiz.");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("En az bir rol atanmalı.")
                .Must(roles => roles.Any(r => r.RoleExist))
                .WithMessage("En az bir rol seçilmelidir.");
        }
    }
}
