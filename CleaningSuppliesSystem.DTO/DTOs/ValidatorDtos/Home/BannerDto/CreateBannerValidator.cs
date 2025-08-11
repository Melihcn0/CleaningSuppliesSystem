using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.BannerDto
{
    public class CreateBannerValidator : AbstractValidator<CreateBannerDto>
    {
        public CreateBannerValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Banner başlığı boş olamaz.")
                .MaximumLength(100).WithMessage("Başlık en fazla 40 karakter olabilir.");

            RuleFor(x => x.Subtitle)
                .NotEmpty().WithMessage("Alt başlık boş olamaz.")
                .MaximumLength(150).WithMessage("Alt başlık en fazla 50 karakter olabilir.");

            RuleFor(x => x.Statistic1Title)
                .NotEmpty().WithMessage("İstatistik 1 adı boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 1 en fazla 15 karakter olabilir.");

            RuleFor(x => x.Statistic1)
                .NotEmpty().WithMessage("İstatistik 1 boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 1 en fazla 7 karakter olabilir.");

            RuleFor(x => x.Statistic2Title)
                .NotEmpty().WithMessage("İstatistik 2 adı boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 2 en fazla 15 karakter olabilir.");

            RuleFor(x => x.Statistic2)
                .NotEmpty().WithMessage("İstatistik 2 boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 2 en fazla 7 karakter olabilir.");

            RuleFor(x => x.Statistic3Title)
                .NotEmpty().WithMessage("İstatistik 3 adı boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 3 en fazla 15 karakter olabilir.");

            RuleFor(x => x.Statistic3)
                .NotEmpty().WithMessage("İstatistik 3 boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 3 en fazla 7 karakter olabilir.");

            RuleFor(x => x.Statistic4Title)
                .NotEmpty().WithMessage("İstatistik 4 adı boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 4 en fazla 15 karakter olabilir.");

            RuleFor(x => x.Statistic4)
                .NotEmpty().WithMessage("İstatistik 4 boş olamaz.")
                .MaximumLength(50).WithMessage("İstatistik 4 en fazla 7 karakter olabilir.");
        }


    }
}
