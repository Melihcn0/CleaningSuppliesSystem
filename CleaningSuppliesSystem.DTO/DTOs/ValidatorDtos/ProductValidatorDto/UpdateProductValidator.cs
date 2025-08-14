using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ProductValidatorDto
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                .MaximumLength(70).WithMessage("Ürün adı en fazla 70 karakter olmalıdır.");

            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Marka seçimi zorunludur.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Ürün birim fiyatı 0'dan büyük olmalı.")
                .LessThanOrEqualTo(5000).WithMessage("Ürün birim fiyatı 5.000₺ geçemez.");

            RuleFor(x => x.VatRate)
                .GreaterThanOrEqualTo(0).WithMessage("KDV oranı 0 veya daha büyük olmalı.")
                .LessThanOrEqualTo(100).WithMessage("KDV 100’ü geçemez.");
        }
    }
}
