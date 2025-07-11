using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Controllers
{
    public class SettingsController(UserManager<AppUser> _userManager, IEmailService _emailService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTwoFactor(bool TwoFactorEnabled)
        {
            var user = await _userManager.GetUserAsync(User);
            await _userManager.SetTwoFactorEnabledAsync(user, TwoFactorEnabled);

            // Kullanıcıya bilgilendirme e-postası gönder
            await _emailService.SendTwoFactorStatusChangedMailAsync(user.UserName, user.Email, TwoFactorEnabled);

            TempData["Message"] = TwoFactorEnabled
                ? "2FA başarıyla açıldı. Bilgilendirme maili gönderildi."
                : "2FA kapatıldı. Kullanıcıya bilgilendirme maili gönderildi.";

            return RedirectToAction("Settings");
        }
    }
}
