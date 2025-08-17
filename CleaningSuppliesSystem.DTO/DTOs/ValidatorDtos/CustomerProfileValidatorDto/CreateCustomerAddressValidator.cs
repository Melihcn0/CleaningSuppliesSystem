using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CustomerProfileValidatorDto
{
    public class CreateCustomerAddressValidator : AbstractValidator<CreateCustomerAddressDto>
    {
        public CreateCustomerAddressValidator()
        {
            RuleFor(x => x.AddressTitle)
                     .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                     .MaximumLength(30).WithMessage("Adres başlığı en fazla 30 karakter olmalıdır.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Açık adres boş bırakılamaz.")
                .MaximumLength(200).WithMessage("Açık adres en fazla 200 karakter olmalıdır.");
        }
    }
}
