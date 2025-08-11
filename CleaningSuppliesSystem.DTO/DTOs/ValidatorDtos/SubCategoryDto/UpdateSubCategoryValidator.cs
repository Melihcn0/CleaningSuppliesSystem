using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.SubCategoryDto
{
    public class UpdateSubCategoryValidator : AbstractValidator<UpdateSubCategoryDto>
    {
        public UpdateSubCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Alt kategori adı boş olamaz.")
                .MinimumLength(2).WithMessage("Alt kategori adı en az 2 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("Alt kategori adı en fazla 50 karakter olabilir.");

            RuleFor(x => x.TopCategoryId)
                .NotNull().WithMessage("Üst kategori seçmelisiniz.");
        }
    }
}
