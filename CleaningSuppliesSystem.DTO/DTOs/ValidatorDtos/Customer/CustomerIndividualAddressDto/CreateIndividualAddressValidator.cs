using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Customer.CustomerIndivualAddressDto
{
    public class CreateIndividualAddressValidator : AbstractValidator<CreateCustomerIndividualAddressDto>
    {
        public CreateIndividualAddressValidator()
        {
            RuleFor(x => x.CityId)
                .NotNull().WithMessage("Şehir seçmelisiniz.");

            RuleFor(x => x.DistrictId)
                .NotEmpty().WithMessage("İlçe seçmelisiniz.");

            RuleFor(x => x.AddressTitle)
                .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                .MaximumLength(35).WithMessage("Adres başlığı en fazla 35 karakter olmalıdır.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                .MaximumLength(150).WithMessage("Adres en fazla 150 karakter olmalıdır.");

        }
    }
}
