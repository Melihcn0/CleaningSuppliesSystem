using CleaningSuppliesSystem.DTO.DTOs.Developer.DeveloperProfileDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.DeveloperProfileValidatorDto
{
    public class UpdateDeveloperProfileValidator : AbstractValidator<UpdateDeveloperProfileDto>
    {
        public UpdateDeveloperProfileValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                .MaximumLength(30).WithMessage("Kullanıcı adı en fazla 30 karakter olabilir.")
                .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Kullanıcı adı yalnızca harf, rakam, nokta, tire ve alt tire içerebilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta giriniz.")
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                .MinimumLength(17).WithMessage("Telefon numarası en az 17 karakter olmalıdır.")
                .MaximumLength(17).WithMessage("Telefon numarası en fazla 17 karakter olmalıdır.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("Kimlik numarası boş bırakılamaz.")
                .MaximumLength(11).WithMessage("Kimlik numarası en fazla 11 karakter olmalıdır.");
        }
    }
}
