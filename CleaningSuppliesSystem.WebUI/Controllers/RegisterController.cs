using CleaningSuppliesSystem.DTO.DTOs.RegisterDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LoginValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RegisterValidatorDto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _client;
        public RegisterController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserRegisterDto userRegisterDto)
        {
            var validator = new RegisterValidator();
            var validationResult = await validator.ValidateAsync(userRegisterDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(userRegisterDto);
            }           
            var result = await _client.PostAsJsonAsync("Users/register", userRegisterDto);
            if (!result.IsSuccessStatusCode)
            {
                var errors = await result.Content.ReadFromJsonAsync<List<RegisterResponseDto>>();
                if (errors != null && errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code ?? "", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bilinmeyen bir hata oluştu.");
                }

                return View(userRegisterDto);
            }
            return RedirectToAction("SignIn", "Login");
        }
    }
}
