using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Services.EmailServices;
using CleaningSuppliesSystem.WebUI.Services.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class RegisterController(IUserService _userService, UserManager<AppUser> _userManager, IEmailService _emailService, IConfiguration _config) : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserRegisterDto userRegisterDto)
        {
            var validator = new UserRegisterValidator();
            var result = await validator.ValidateAsync(userRegisterDto);

            if (!result.IsValid)
            {
                foreach (var x in result.Errors)
                {
                    ModelState.Remove(x.PropertyName);
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
                return View(userRegisterDto);
            }

            // 👤 AppUser oluşturuluyor
            var user = new AppUser
            {
                UserName = userRegisterDto.UserName,
                Email = userRegisterDto.Email,
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                IsActive = false,
                LockoutEnabled = true
            };

            // ✏️ Şifre uyuşmazlığı kontrolü
            if (userRegisterDto.Password != userRegisterDto.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Şifre ve tekrar şifresi uyuşmuyor.");
                return View(userRegisterDto);
            }

            // 📌 Kullanıcı oluşturuluyor
            var identityResult = await _userManager.CreateAsync(user, userRegisterDto.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var x in identityResult.Errors)
                {
                    switch (x.Code)
                    {
                        case "DuplicateUserName":
                            ModelState.AddModelError("UserName", x.Description);
                            break;
                        case "DuplicateEmail":
                            ModelState.AddModelError("Email", x.Description);
                            break;
                        case "PasswordMismatch":
                            ModelState.AddModelError("ConfirmPassword", x.Description);
                            break;
                        default:
                            ModelState.AddModelError("", x.Description);
                            break;
                    }
                }
                return View(userRegisterDto);
            }

            // 🏷️ Kullanıcı rolü atanıyor
            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", $"Rol ataması başarısız: {error.Description}");
                }
                return View(userRegisterDto);
            }

            // 📬 E-posta servisinden çağrım
            await _emailService.NewUserMailAsync(user.UserName, user.Email);
            await _emailService.SendUserWelcomeMailAsync(user.UserName, user.Email);

            // 🔄 Login sayfasına yönlendirme
            return RedirectToAction("SignIn", "Login");
        }
    }
}
