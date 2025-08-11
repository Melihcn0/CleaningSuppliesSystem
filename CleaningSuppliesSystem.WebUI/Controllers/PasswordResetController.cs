using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ForgotPasswordValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LoginValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ResetPasswordValidatorDto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class PasswordResetController : Controller
    {
        private readonly HttpClient _client;
        public PasswordResetController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(UserForgotPasswordDto userforgotPasswordDto)
        {
            var validator = new ForgotPasswordValidator();
            var validationResult = await validator.ValidateAsync(userforgotPasswordDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(userforgotPasswordDto);
            }            
            var result = await _client.PostAsJsonAsync("Users/forgot-Password", userforgotPasswordDto);
            if (result.IsSuccessStatusCode)
            {
                TempData["Success"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
                return RedirectToAction("Index");
            }
            return View("Index", userforgotPasswordDto);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            var validator = new ResetPasswordValidator();
            var validationResult = await validator.ValidateAsync(userResetPasswordDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(userResetPasswordDto);
            }           
            var response = await _client.PostAsJsonAsync("Users/reset-password", userResetPasswordDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Şifreniz başarıyla güncellendi.";
                return RedirectToAction("ResetForm");
            }
            var apiErrors = await response.Content.ReadFromJsonAsync<List<string>>();
            if (apiErrors != null)
            {
                foreach (var error in apiErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Bilinmeyen bir hata oluştu.");
            }
            return View("ResetForm", userResetPasswordDto);
        }

        [HttpGet]
        public IActionResult ResetForm(string? token, string? email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Şifre sıfırlama bağlantısı eksik veya geçersiz.");
                return View("Index");
            }

            var dto = new UserResetPasswordDto
            {
                Token = token,
                Email = email
            };

            return View("ResetForm", dto);
        }
    }
}
