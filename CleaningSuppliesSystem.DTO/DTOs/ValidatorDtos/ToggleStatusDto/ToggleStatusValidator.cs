using CleaningSuppliesSystem.DTO.DTOs.ToggleDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ToggleStatusValidatorDto
{
    public class ToggleStatusValidator : AbstractValidator<ToggleStatusDto>
    {
        public ToggleStatusValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Geçersiz ID");
        }
    }
}
