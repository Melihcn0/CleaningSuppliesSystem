using CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.PromoAlertValidatorDto
{
    public class UpdatePromoAlertValidator : AbstractValidator<UpdatePromoAlertDto>
    {
        public UpdatePromoAlertValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Başlık en fazla 50 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Açıklama en fazla 100 karakter olabilir.");

            RuleFor(x => x.Icon)
                .NotEmpty().WithMessage("Lütfen bir ikon seçin.")
                .Must(icon => new[] { "question", "success", "warning", "error", "info" }.Contains(icon))
                .WithMessage("Geçersiz ikon seçimi.");
        }
    }
}
