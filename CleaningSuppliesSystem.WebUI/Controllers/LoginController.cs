using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CleaningSuppliesSystem.DTO.DTOs.LoginDtos;
using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.LoginValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.TokenDtos;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;

namespace CleaningSuppliesSystem.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _client;
        public LoginController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserLoginDto userLoginDto)
        {
            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(userLoginDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(userLoginDto);
            }

            var responseMessage = await _client.PostAsJsonAsync("Users/login", userLoginDto);

            if (!responseMessage.IsSuccessStatusCode)
            {
                var errorResponse = await responseMessage.Content.ReadFromJsonAsync<List<TokenDetailDto>>();
                if (errorResponse != null && errorResponse.Any())
                {
                    var actualErrors = errorResponse
                        .Where(e => !string.Equals(e.Code?.Trim(), "RemainingAttempts", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    ViewBag.HataSayisi = actualErrors.Count;

                    foreach (var error in actualErrors)
                    {
                        var key = error.Code?.Trim() switch
                        {
                            "AccountStatus" or "Identifier" or "AccountLockout" => "Identifier",
                            "Password" => "Password",
                            _ => error.Code ?? ""
                        };

                        ModelState.AddModelError(key, error.Description ?? "Giriş işlemi başarısız.");
                    }

                    var remainingError = errorResponse
                        .FirstOrDefault(e => string.Equals(e.Code?.Trim(), "RemainingAttempts", StringComparison.OrdinalIgnoreCase));

                    if (remainingError != null && !string.IsNullOrWhiteSpace(remainingError.Description))
                    {
                        const int maxAttempts = 10;

                        if (int.TryParse(remainingError.Description.Trim(), out int remaining))
                        {
                            int used = maxAttempts - remaining;
                            ViewBag.RemainingAttempts = remaining;
                            ViewBag.RemainingAttemptsMessage = $"Toplam {maxAttempts} denemeden {used} tanesi kullanıldı. Kalan: {remaining}";
                        }
                        else
                        {
                            ViewBag.RemainingAttemptsMessage = remainingError.Description;
                        }
                    }
                }

                return View(userLoginDto);
            }

            var response = await responseMessage.Content.ReadFromJsonAsync<LoginResponseDto>();
            var jwtToken = response?.Token?.Token?.Token;
            var expireDate = response?.Token?.Token?.ExpireDate;

            if (string.IsNullOrEmpty(jwtToken) || expireDate == null)
            {
                ModelState.AddModelError("", "Token alınamadı.");
                return View(userLoginDto);
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            // 🔍 Token içindeki claim’leri al
            var name = token.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == ClaimTypes.Name)?.Value ?? userLoginDto.Identifier;
            var role = token.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value ?? "Customer";
            var theme = response?.Theme ?? "light";



            // 🔐 Identifier sabit olarak DTO'dan alınıyor
            var identifier = userLoginDto.Identifier;

            // ✅ ClaimsPrincipal oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, identifier),
                new Claim("Identifier", identifier),
                new Claim("theme", theme) // 🔥 Tema bilgisi burada

            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProps = new AuthenticationProperties
            {
                ExpiresUtc = expireDate,
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProps);

            // 🔐 Token'ı HttpOnly cookie olarak sakla (TokenHandler için garanti)
            Response.Cookies.Append("AccessToken", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expireDate
            });

            // 🔁 Role’a göre yönlendirme
            return role switch
            {
                "Admin" => RedirectToAction("Index", "LocationCity", new { area = "Admin" }),
                "Customer" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("AccessDenied", "ErrorPage")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var response = await _client.PostAsync("users/logout", new StringContent(""));

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
