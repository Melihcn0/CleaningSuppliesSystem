using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.WebUI.Services.UserServices;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class LoginController(IUserService _userService) : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserLoginDto userLoginDto)
        {
            // 🧩 FluentValidation
            var validator = new UserLoginValidator();
            var result = await validator.ValidateAsync(userLoginDto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(userLoginDto);
            }

            // 🔐 Servisten login sonucu al
            var loginResult = await _userService.LoginUserAsync(userLoginDto);

            if (!loginResult.Succeeded)
            {
                foreach (var error in loginResult.Errors)
                {
                    var field = error.Code switch
                    {
                        "UserNotFound" => "Identifier",
                        "UserInactive" => "Identifier",
                        "UserLocked" => "Identifier",
                        "InvalidIdentifierFormat" => "Identifier",
                        "InvalidPassword" => "Password",
                        "PasswordMismatch" => "Password",
                        _ => ""
                    };

                    if (error.Code == "UserInactive")
                    {
                        TempData["InactiveUser"] = true;
                        TempData["AlertType"] = "warning";
                        TempData["AlertMessage"] = "Hesabınız pasif durumda. Yetkiliye bildirim gönderildi.";
                    }

                    if (error.Code == "LoginAttemptsExhausted")
                    {
                        TempData["AlertType"] = "error";
                        TempData["AlertMessage"] = "Tüm giriş deneme haklarınız doldu. Lütfen 30 dakika sonra tekrar deneyin.";
                    }

                    ModelState.AddModelError(field, error.Description);
                }

                var remainingAttemptStr = loginResult.Errors
                    .FirstOrDefault(e => e.Code == "RemainingAttempts")?.Description;

                if (int.TryParse(remainingAttemptStr, out int remainingAttempt))
                {
                    int usedAttempt = 10 - remainingAttempt;
                    ViewBag.RemainingAttempts = $"Toplam 10 denemeden {usedAttempt} tanesi kullanıldı. Kalan: {remainingAttempt}";
                }

                return View(userLoginDto);
            }

            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Giriş başarılı. Hoş geldiniz!";

            return loginResult.Role switch
            {
                "Admin" => RedirectToAction("Index", "Category", new { area = "Admin" }),
                "Customer" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };

        }
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
