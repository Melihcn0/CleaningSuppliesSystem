using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Customer.CustomerCorporateAddressDto
{
    public class CreateCorporateAddressValidator : AbstractValidator<CreateCustomerCorporateAddressDto>
    {
        public CreateCorporateAddressValidator()
        {
            RuleFor(x => x.CityId)
                .NotNull().WithMessage("Şehir seçmelisiniz.");

            RuleFor(x => x.DistrictId)
                .NotEmpty().WithMessage("İlçe seçmelisiniz.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Şirket adı en fazla 100 karakter olmalıdır.");

            RuleFor(x => x.TaxOffice)
                .NotEmpty().WithMessage("Vergi dairesi boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Vergi dairesi 100 karakter olmalıdır.");

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Vergi numarası boş bırakılamaz.")
                .Length(11).WithMessage("Vergi numarası 11 karakter olmalıdır.");

            RuleFor(x => x.AddressTitle)
                .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                .MaximumLength(35).WithMessage("Adres başlığı en fazla 35 karakter olmalıdır.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");
        }
    }
}
