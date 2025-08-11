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

            if (responseMessage.IsSuccessStatusCode)
            {
                var response = await responseMessage.Content.ReadFromJsonAsync<LoginResponseDto>();

                if (!string.IsNullOrEmpty(response?.Token?.Token?.Token))
                {
                    string jwtToken = response.Token.Token.Token;
                    DateTime expireDate = response.Token.Token.ExpireDate;

                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwtToken);

                    var claims = token.Claims.ToList();
                    claims.Add(new Claim("Token", jwtToken));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProps = new AuthenticationProperties
                    {
                        ExpiresUtc = expireDate,
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProps);

                    // ---- Session'a kullanıcı adı ve rol ekleme ----
                    HttpContext.Session.SetString("Identifier", userLoginDto.Identifier);

                    var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
                    if (roleClaim != null)
                    {
                        HttpContext.Session.SetString("UserRole", roleClaim.Value);
                    }
                    // ----------------------------------------------

                    var userRole = roleClaim?.Value;

                    return userRole switch
                    {
                        "Admin" => RedirectToAction("Index", "SecondaryBanner", new { area = "Admin" }),
                        "Customer" => RedirectToAction("Index", "Home"),
                        _ => RedirectToAction("AccessDenied", "Auth")
                    };
                }

                ModelState.AddModelError("", "Token alınamadı.");
                return View(userLoginDto);
            }

            // Hata yönetimi (senin mevcut kodun)
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




        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
