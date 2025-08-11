using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.ServiceValidatorDto
{
    public class UpdateServiceValidator : AbstractValidator<UpdateServiceDto>
    {
        public UpdateServiceValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Hizmet adı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Hizmet adı en fazla 50 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Hizmet açıklaması boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Hizmet açıklaması en fazla 100 karakter olmalıdır.");

            RuleFor(x => x.ServiceIconId)
                .NotNull().WithMessage("Hizmet iconu seçmelisiniz.");
        }
    }
}
