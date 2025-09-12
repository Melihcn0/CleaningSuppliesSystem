using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CompanyAddressValidatorDto
{
    public class UpdateCompanyAddressValidator : AbstractValidator<UpdateCompanyAddressDto>
    {
        public UpdateCompanyAddressValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olmalıdır.");

            RuleFor(x => x.TaxOffice)
                .NotEmpty().WithMessage("Vergi dairesi boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Vergi dairesi 100 karakter olmalıdır.");

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Vergi numarası boş bırakılamaz.")
                .Length(11).WithMessage("Vergi numarası 11 karakter olmalıdır.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");

            RuleFor(x => x.CityName)
                .NotEmpty().WithMessage("Şehir adı boş bırakılamaz.")
                .MaximumLength(15).WithMessage("Şehir adı en fazla 15 karakter olmalıdır.");

            RuleFor(x => x.DistrictName)
                .NotEmpty().WithMessage("İlçe adı boş bırakılamaz.")
                .MaximumLength(25).WithMessage("İlçe adı en fazla 25 karakter olmalıdır.");
        }
    }
}
