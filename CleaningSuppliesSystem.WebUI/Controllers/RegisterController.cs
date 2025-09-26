using CleaningSuppliesSystem.DTO.DTOs.RegisterDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LoginValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.RegisterValidatorDto;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            try
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
                
                userRegisterDto.Email = userRegisterDto.Email?.ToLower();
                userRegisterDto.UserName = ConvertTurkishCharsToEnglish(userRegisterDto.UserName).ToUpper();
                userRegisterDto.FirstName = userRegisterDto.FirstName?
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower())
                    .Aggregate((a, b) => a + " " + b);

                userRegisterDto.LastName = userRegisterDto.LastName?
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower())
                    .Aggregate((a, b) => a + " " + b);


                var result = await _client.PostAsJsonAsync("Users/register", userRegisterDto);

                if (!result.IsSuccessStatusCode)
                {
                    // Response'u string olarak al
                    var content = await result.Content.ReadAsStringAsync();

                    try
                    {
                        // JSON dönmüşse deserialize et
                        var errors = JsonSerializer.Deserialize<List<RegisterResponseDto>>(content);

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
                    }
                    catch (JsonException)
                    {
                        // JSON değilse, direkt text mesajı göster
                        ModelState.AddModelError("", content);
                    }

                    return View(userRegisterDto);
                }

                return RedirectToAction("SignIn", "Login");
            }
            catch (Exception ex)
            {
                // loglamak mantıklı olur
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                return View(userRegisterDto);
            }
        }

        private string ConvertTurkishCharsToEnglish(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return input
                .Replace("ç", "c").Replace("Ç", "C")
                .Replace("ğ", "g").Replace("Ğ", "G")
                .Replace("ı", "I").Replace("İ", "I")
                .Replace("ö", "o").Replace("Ö", "O")
                .Replace("ş", "s").Replace("Ş", "S")
                .Replace("ü", "u").Replace("Ü", "U");
        }


    }
}
