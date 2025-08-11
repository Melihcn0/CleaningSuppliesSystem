using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.DiscountValidatorDto
{
    public class UpdateDiscountValidator : AbstractValidator<UpdateDiscountDto>
    {
        public UpdateDiscountValidator()
        {
            RuleFor(x => x.DiscountRate)
                .GreaterThanOrEqualTo(0).WithMessage("İndirim oranı 0 veya daha büyük olmalı.")
                .LessThanOrEqualTo(100).WithMessage("İndirim %100’ü geçemez.");
        }
    }
}
