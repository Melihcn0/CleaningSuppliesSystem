using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.BrandValidatorDto
{
    public class CreateBrandValidator : AbstractValidator<CreateBrandDto>
    {
        public CreateBrandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("Kategori seçmelisiniz.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Marka adı boş olamaz.")
                .MinimumLength(2).WithMessage("Marka adı en az 2 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("Marka adı en fazla 50 karakter olabilir.");
        }
    }
}
