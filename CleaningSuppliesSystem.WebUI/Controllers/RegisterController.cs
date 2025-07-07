using CleaningSuppliesSystem.WebUI.DTOs.UserDtos;
using CleaningSuppliesSystem.WebUI.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static CleaningSuppliesSystem.WebUI.Validators.Validators;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class RegisterController(IUserService _userService) : Controller
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

            var identityResult = await _userService.CreateUserAsync(userRegisterDto);

            if (!identityResult.Succeeded)
            {
                foreach (var x in identityResult.Errors)
                {
                    ModelState.AddModelError("", x.Description);
                }
                return View(userRegisterDto);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
