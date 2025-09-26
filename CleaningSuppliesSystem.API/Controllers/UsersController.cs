using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.ErrorDetailDtos;
using CleaningSuppliesSystem.DTO.DTOs.ForgotPasswordDtos;
using CleaningSuppliesSystem.DTO.DTOs.LoginDtos;
using CleaningSuppliesSystem.DTO.DTOs.RegisterDtos;
using CleaningSuppliesSystem.DTO.DTOs.ResetPasswordDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(GroupName = "User")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<ForgotPasswordDto> _forgotPasswordValidator;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;

    public UsersController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtService jwtService,
        IMapper mapper,
        IEmailService emailService,
        IValidator<LoginDto> loginValidator,
        IValidator<RegisterDto> registerValidator,
        IValidator<ForgotPasswordDto> forgotPasswordValidator,
        IValidator<ResetPasswordDto> resetPasswordValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _mapper = mapper;
        _emailService = emailService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _forgotPasswordValidator = forgotPasswordValidator;
        _resetPasswordValidator = resetPasswordValidator;
    }

    [HttpGet("check-token")]
    public IActionResult CheckToken()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var cookieToken = Request.Cookies["AccessToken"];

        return Ok(new
        {
            AuthorizationHeader = authHeader,
            AccessTokenCookie = cookieToken
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Identifier))
        {
            return BadRequest(new List<ErrorDetailDto>
        {
            new ErrorDetailDto { Code = "Identifier", Description = "Kullanıcı adı veya email boş olamaz." }
        });
        }

        loginDto.Identifier = ConvertTurkishCharsToEnglish(loginDto.Identifier);

        var validationResult = await _loginValidator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors
                .Select(e => new ErrorDetailDto { Code = e.PropertyName, Description = e.ErrorMessage })
                .ToList();
            return BadRequest(validationErrors);
        }

        var errors = new List<ErrorDetailDto>();

        // ✅ Email -> lowercase, Username -> Identity normalize
        AppUser user = null;
        if (loginDto.Identifier.Contains("@"))
            user = await _userManager.FindByEmailAsync(loginDto.Identifier.ToLower());
        else
            user = await _userManager.FindByNameAsync(_userManager.NormalizeName(loginDto.Identifier));

        if (user == null)
        {
            errors.Add(new ErrorDetailDto
            {
                Code = "Identifier",
                Description = "Kullanıcı adı veya email sisteme kayıtlı değil."
            });
            return BadRequest(errors);
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, true);

        if (!result.Succeeded)
        {
            var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
            var maxAttempts = 10;
            var remaining = Math.Max(maxAttempts - accessFailedCount, 0);

            errors.Add(new ErrorDetailDto { Code = "RemainingAttempts", Description = remaining.ToString() });

            if (result.IsLockedOut)
            {
                errors.Add(new ErrorDetailDto
                {
                    Code = "AccountLockout",
                    Description = "Hesabınız geçici olarak kilitlendi. Lütfen 30 dakika sonra tekrar deneyin."
                });
            }
            else
            {
                errors.Add(new ErrorDetailDto
                {
                    Code = "Password",
                    Description = "Kullanıcı adı, email veya şifre hatalı."
                });
            }

            return BadRequest(errors);
        }

        if (!user.IsActive)
        {
            await _emailService.PassiveUserLoginMailAsync(user.UserName, user.Email);

            errors.Add(new ErrorDetailDto
            {
                Code = "AccountStatus",
                Description = "Kullanıcı hesabı pasif durumda. Yöneticiye bilgi verildi."
            });
            return BadRequest(errors);
        }

        var token = await _jwtService.CreateTokenAsync(user);
        return Ok(token);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        try
        {
            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new ErrorDetailDto
                    {
                        Code = e.PropertyName,
                        Description = e.ErrorMessage ?? "Geçersiz değer."
                    }).ToList();

                return BadRequest(errors);
            }

            var user = _mapper.Map<AppUser>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var identityErrors = result.Errors
                    .Select(e => new ErrorDetailDto
                    {
                        Code = e.Code,
                        Description = e.Description ?? "Bir hata oluştu."
                    }).ToList();

                return BadRequest(identityErrors);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            await _emailService.NewUserMailAsync(user.UserName, user.Email);
            await _emailService.SendUserWelcomeMailAsync(user.UserName, user.Email);

            return Ok(new { message = "Kayıt Başarılı" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Register hata: " + ex.Message);
            return BadRequest(new
            {
                Code = "Exception",
                Description = ex.Message
            });
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



    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPasswordDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(err => new ErrorDetailDto
                {
                    Code = err.PropertyName,
                    Description = err.ErrorMessage
                }).ToList();
            return BadRequest(errors);
        }

        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            return BadRequest(new List<ErrorDetailDto>
            {
                new ErrorDetailDto
                {
                    Code = "Email",
                    Description = "Bu e-posta ile ilişkili kullanıcı bulunamadı."
                }
            });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetMailLinkAsync(user.UserName, user.Email, token);
        return Ok(new { message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var validationResult = await _resetPasswordValidator.ValidateAsync(resetPasswordDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(err => new ErrorDetailDto
                {
                    Code = err.PropertyName,
                    Description = err.ErrorMessage
                }).ToList();
            return BadRequest(errors);
        }

        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return BadRequest(new List<ErrorDetailDto>
            {
                new ErrorDetailDto
                {
                    Code = "Email",
                    Description = "Bu e-posta ile ilişkili kullanıcı bulunamadı."
                }
            });
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new ErrorDetailDto
                {
                    Code = "ResetError",
                    Description = e.Description
                }).ToList();

            return BadRequest(errors);
        }

        await _emailService.SendPasswordResetMailAsync(user.UserName, user.Email);
        return Ok(new { message = "Şifreniz başarıyla güncellendi." });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            user.LastLogoutAt = DateTime.Now;
            await _userManager.UpdateAsync(user);
        }
        await _signInManager.SignOutAsync();
        return Ok();
    }
}