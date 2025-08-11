using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.FinanceValidatorDto
{
    public class UpdateFinanceValidator : AbstractValidator<UpdateFinanceDto>
    {
        public UpdateFinanceValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Finans tipi seçimi zorunludur.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Finans adı boş bırakılamaz.")
                .MaximumLength(40).WithMessage("Ürün adı en fazla 40 karakter olmalıdır.");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Finans miktarı boş bırakılamaz.")
                .GreaterThan(0).WithMessage("Finans miktarı sıfırdan büyük olmalıdır.")
                .LessThanOrEqualTo(10000).WithMessage("Finans miktarı 10000 TL'den fazla olamaz.");
        }
    }
}
