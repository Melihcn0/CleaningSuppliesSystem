using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.SecondaryBannerDto
{
    public class UpdateSecondaryBannerValidator : AbstractValidator<UpdateSecondaryBannerDto>
    {
        public UpdateSecondaryBannerValidator()
        {
            RuleFor(x => x.Title1)
                .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

            RuleFor(x => x.Title2)
                .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

            RuleFor(x => x.Title3)
                .NotEmpty().WithMessage("İkincil banner başlığı boş olamaz.")
                .MaximumLength(15).WithMessage("İkincil banner Başlığı en fazla 15 karakter olabilir.");

            RuleFor(x => x.Description1)
                .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

            RuleFor(x => x.Description2)
                .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

            RuleFor(x => x.Description3)
                .NotEmpty().WithMessage("İkincil banner açıklaması boş olamaz.")
                .MaximumLength(25).WithMessage("İkincil banner açıklaması en fazla 25 karakter olabilir.");

            RuleFor(x => x.ButtonTitle1)
                .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

            RuleFor(x => x.ButtonTitle2)
                .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");

            RuleFor(x => x.ButtonTitle3)
                .NotEmpty().WithMessage("İkincil banner buton adı boş olamaz.")
                .MaximumLength(10).WithMessage("İkincil banner butonunun adı en fazla 10 karakter olabilir.");
        }
    }
}
