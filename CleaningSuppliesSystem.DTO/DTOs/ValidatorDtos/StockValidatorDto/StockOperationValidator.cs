using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.StockValidatorDto
{
    public class CreateStockOperationValidator : AbstractValidator<CreateStockOperationDto>
    {
        public CreateStockOperationValidator()
        {
            RuleFor(x => x.TopCategoryId)
                .GreaterThan(0).WithMessage("Üst kategori seçmelisiniz.");

            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("Alt kategori seçmelisiniz.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Kategori seçmelisiniz.");

            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Marka seçmelisiniz.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Ürün seçmelisiniz.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(100).WithMessage("Miktar 100'e eşit veya 100'den küçük olmalıdır.");
        }
    }
}
