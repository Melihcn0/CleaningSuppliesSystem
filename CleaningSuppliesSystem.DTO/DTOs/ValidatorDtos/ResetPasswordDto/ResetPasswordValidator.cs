using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ResetPasswordValidatorDto
{
    public class ResetPasswordValidator : AbstractValidator<UserResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
                .MaximumLength(50).WithMessage("Email en fazla 50 karakter olabilir.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olabilir.")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar alanı boş bırakılamaz.")
                .Equal(x => x.NewPassword).WithMessage("Şifreler birbiriyle uyumlu değil.")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Şifre en az bir özel karakter içermelidir.");
        }
    }
}
