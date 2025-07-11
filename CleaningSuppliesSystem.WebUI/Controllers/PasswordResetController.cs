using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.WebUI.Services.EmailServices;
using CleaningSuppliesSystem.WebUI.Services.UserServices;
using FluentValidation;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using static CleaningSuppliesSystem.Business.Validators.Validators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class PasswordResetController(IUserService _userService, IEmailService _emailService, IValidator<ForgotPasswordDto> _forgotPasswordValidator, IValidator<ResetPasswordDto> _resetPasswordValidator) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateToken(ForgotPasswordDto forgotPasswordDto)
        {
            var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPasswordDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError("Email", error.ErrorMessage);
                }
                return View("Index", forgotPasswordDto);
            }

            var user = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Bu e-posta ile ilişkili kullanıcı bulunamadı.");
                // Maili sıfırlama işlemini kaldırıyoruz
                return View("Index", forgotPasswordDto);
            }

            var token = await _userService.GeneratePasswordResetTokenByIdAsync(user.Id);

            await _emailService.SendPasswordResetMailLinkAsync(user.UserName, user.Email, token);

            TempData["Success"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View("ResetForm", dto);
            }

            var user = await _userService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "E-posta adresine ait kullanıcı bulunamadı.");
                return View("ResetForm", dto);
            }

            var result = await _userService.ResetPasswordByIdAsync(user.Id, dto.Token, dto.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Şifreniz başarıyla güncellendi.";
                await _emailService.SendPasswordResetMailAsync(user.UserName, user.Email);
                return RedirectToAction("ResetForm");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("ResetForm", dto);
        }

        [HttpGet]
        public IActionResult ResetForm(string? token, string? email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Şifre sıfırlama bağlantısı eksik veya geçersiz.");
                return View("Index");
            }

            var dto = new ResetPasswordDto
            {
                Token = token,
                Email = email
            };

            return View(dto);
        }


    }
}
