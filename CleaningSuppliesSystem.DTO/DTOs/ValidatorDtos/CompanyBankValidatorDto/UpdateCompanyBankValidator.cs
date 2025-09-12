using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CompanyBankValidatorDto
{
    public class UpdateCompanyBankValidator : AbstractValidator<UpdateCompanyBankDto>
    {
        public UpdateCompanyBankValidator()
        {
            RuleFor(x => x.BankName)
                .NotEmpty().WithMessage("Banka adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Banka adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.AccountHolder)
                .NotEmpty().WithMessage("Hesap sahibi boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Hesap sahibi en fazla 100 karakter olabilir.");

            RuleFor(x => x.IBAN)
                .NotEmpty().WithMessage("IBAN boş bırakılamaz.")
                .Length(26).WithMessage("Türkiye IBAN'ı 26 karakter olmalı.");
        }
    }
}
