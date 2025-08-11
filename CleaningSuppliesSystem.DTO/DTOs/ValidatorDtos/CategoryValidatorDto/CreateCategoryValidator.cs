using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CategoryValidatorDto
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Kategori adı en fazla 50 karakter olmalıdır.");

            RuleFor(x => x.SubCategoryId)
                .NotNull().WithMessage("Alt kategori seçmelisiniz.");

            RuleFor(x => x.TopCategoryId)
                .NotNull().WithMessage("Üst kategori seçmelisiniz.");
        }
    }
}
