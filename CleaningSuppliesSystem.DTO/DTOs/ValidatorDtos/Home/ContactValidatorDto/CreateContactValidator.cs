using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.ContactValidatorDto
{
    public class CreateContactValidator : AbstractValidator<CreateContactDto>
    {
        public CreateContactValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                .MinimumLength(17).WithMessage("Telefon numarası en az 17 karakter olmalıdır.")
                .MaximumLength(17).WithMessage("Telefon numarası en fazla 17 karakter olmalıdır.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Mesaj boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Mesaj en fazla 100 karakter olabilir.");
        }
    }
}
