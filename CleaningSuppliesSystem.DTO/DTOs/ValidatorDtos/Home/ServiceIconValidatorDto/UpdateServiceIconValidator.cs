using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.ServiceIconValidatorDto
{
    public class UpdateServiceIconValidator : AbstractValidator<UpdateServiceIconDto>
    {
        public UpdateServiceIconValidator()
        {
            RuleFor(x => x.IconName)
                .NotEmpty().WithMessage("Icon adı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Icon adı en fazla 50 karakter olmalıdır.");

            RuleFor(x => x.IconUrl)
                .NotNull().WithMessage("Icon Url boş bırakılamaz.")
                .MaximumLength(70).WithMessage("Icon Url en fazla 70 karakter olmalıdır.");
        }
    }
}
