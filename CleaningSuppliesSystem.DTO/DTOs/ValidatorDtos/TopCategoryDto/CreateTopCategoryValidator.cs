using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.TopCategoryDto
{
    public class CreateTopCategoryValidator : AbstractValidator<CreateTopCategoryDto>
    {
        public CreateTopCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Üst kategori adı boş olamaz.")
                .MinimumLength(2).WithMessage("Üst kategori adı en az 2 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("AÜstlt kategori adı en fazla 50 karakter olabilir.");
        }
    }
}
