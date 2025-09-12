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
using CleaningSuppliesSystem.DTO.DTOs.ErrorDetailDtos;

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
            var sessionWarning = HttpContext.Session.GetString("SessionExpiredWarning");
            if (!string.IsNullOrEmpty(sessionWarning))
            {
                TempData["SessionExpiredWarning"] = sessionWarning;
                HttpContext.Session.Remove("SessionExpiredWarning");
            }
            return View();
        }

        // POST: Login işlemi
        [HttpPost]
        public async Task<IActionResult> SignIn(UserLoginDto userLoginDto)
        {
            // Session'dan uyarı mesajını TempData'ya taşı (POST durumunda da)
            var sessionWarning = HttpContext.Session.GetString("SessionExpiredWarning");
            if (!string.IsNullOrEmpty(sessionWarning))
            {
                TempData["SessionExpiredWarning"] = sessionWarning;
                HttpContext.Session.Remove("SessionExpiredWarning");
            }

            // 🔹 Kullanıcı giriş verilerini doğrula
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

            // 🔹 API'ye login isteği gönder
            var responseMessage = await _client.PostAsJsonAsync("Users/login", userLoginDto);

            if (!responseMessage.IsSuccessStatusCode)
            {
                // 🔹 API hatalarını işle
                var errorResponse = await responseMessage.Content.ReadFromJsonAsync<List<ErrorDetailDto>>();
                if (errorResponse != null && errorResponse.Any())
                {
                    // 🔹 Hataları filtrele (RemainingAttempts hariç)
                    var actualErrors = errorResponse
                        .Where(e => !string.Equals(e.Code?.Trim(), "RemainingAttempts", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    ViewBag.ErrorCount = actualErrors.Count;

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

                    // 🔹 Kalan deneme sayısını al ve ViewBag'e ekle
                    var remainingError = errorResponse
                        .FirstOrDefault(e => string.Equals(e.Code?.Trim(), "RemainingAttempts", StringComparison.OrdinalIgnoreCase));

                    if (remainingError != null && !string.IsNullOrWhiteSpace(remainingError.Description))
                    {
                        const int maxAttempts = 10;

                        if (int.TryParse(remainingError.Description.Trim(), out int remaining))
                        {
                            int used = maxAttempts - remaining;
                            ViewBag.RemainingAttempts = remaining;
                            ViewBag.RemainingAttemptsMessage = $"Toplam {maxAttempts} denemeden {used} tanesi kullanıldı. Kalan: {remaining}.";
                        }
                        else
                        {
                            ViewBag.RemainingAttemptsMessage = remainingError.Description;
                        }
                    }
                }

                return View(userLoginDto);
            }

            // 🔹 API başarılı ise response al
            var response = await responseMessage.Content.ReadFromJsonAsync<LoginResponseDto>();

            // 🔹 Tek seviye token yapısı
            var jwtToken = response?.Token;
            var expireDate = response?.ExpireDate;
            var isActive = response?.IsActive ?? false;

            if (string.IsNullOrEmpty(jwtToken) || expireDate == null || !isActive)
            {
                ModelState.AddModelError("", isActive ? "Token alınamadı." : "Kullanıcı hesabı pasif durumda.");
                return View(userLoginDto);
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            // 🔹 Token içindeki claim'leri al
            var name = token.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == ClaimTypes.Name)?.Value ?? userLoginDto.Identifier;
            var role = token.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value ?? "Customer";
            var theme = response?.Theme ?? "light";

            // 🔹 Claims oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userLoginDto.Identifier),
                new Claim("Identifier", userLoginDto.Identifier),
                new Claim("theme", theme)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProps = new AuthenticationProperties
            {
                ExpiresUtc = expireDate,
                IsPersistent = true
            };

            // 🔹 Cookie Authentication ile login
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProps);

            // 🔹 HttpOnly cookie olarak token sakla
            Response.Cookies.Append("AccessToken", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expireDate
            });

            // 🔁 Role'a göre yönlendirme
            return role switch
            {
                "Admin" => RedirectToAction("Index", "AdminProfile", new { area = "Admin" }),
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