using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Services.UserServices
{
    public class UserService(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<AppRole> _roleManager, IEmailService _emailService, IConfiguration _config) : IUserService
    {
        public async Task<IdentityResult> CreateUserAsync(UserRegisterDto userRegisterDto)
        {
            var user = new AppUser
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                UserName = userRegisterDto.UserName,
                Email = userRegisterDto.Email,
            };

            var errors = new List<IdentityError>();

            if (userRegisterDto.Password != userRegisterDto.ConfirmPassword)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordMismatch",
                    Description = "Şifre ve şifre tekrarı eşleşmiyor.",
                });
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(userRegisterDto.UserName);
            if (existingUserByUsername != null)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = "Bu kullanıcı adı zaten kullanılıyor."
                });
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(userRegisterDto.Email);
            if (existingUserByEmail != null)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Bu e-posta adresi zaten kayıtlı."
                });
            }

            if (errors.Any())
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

            if (!result.Succeeded)
            {
                return IdentityResult.Failed(result.Errors.ToArray());
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded)
            {
                return IdentityResult.Failed(roleResult.Errors.ToArray());
            }

            return result;
        }
        public async Task<LoginResultDto> LoginUserAsync(UserLoginDto userLoginDto)
        {
            var errors = new List<IdentityError>();
            var adminMail = _config["Mail:AdminReceiver"];

            var user = await _userManager.FindByEmailAsync(userLoginDto.Identifier)
                     ?? await _userManager.FindByNameAsync(userLoginDto.Identifier);

            if (user == null)
            {
                errors.Add(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "Email veya kullanıcı adı geçersiz."
                });

                return new LoginResultDto { Succeeded = false, Errors = errors };
            }

            if (!user.IsActive)
            {
                await _emailService.PassiveUserLoginAlertAsync(user.UserName, user.Email);

                errors.Add(new IdentityError
                {
                    Code = "UserInactive",
                    Description = "Hesabınız pasif durumda. Yetkiliye bildirim gönderildi."
                });

                return new LoginResultDto { Succeeded = false, Errors = errors };
            }


            if (await _userManager.IsLockedOutAsync(user))
            {
                errors.Add(new IdentityError
                {
                    Code = "UserLocked",
                    Description = "Hesabınız çok sayıda hatalı giriş nedeniyle kilitlendi. 30 Dakika sonra tekrar deneyin."
                });

                return new LoginResultDto { Succeeded = false, Errors = errors };
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password, false, true);

            if (!signInResult.Succeeded)
            {
                // ❗ Bu satırdan sonra deneme sayısı artmış olmalı (Identity ayarları doğruysa)
                var userId = user.Id.ToString();
                var updatedUser = await _userManager.FindByIdAsync(userId);
                int failedCount = await _userManager.GetAccessFailedCountAsync(updatedUser);
                int remaining = Math.Max(0, 10 - failedCount); // negatif kalmasın

                errors.Add(new IdentityError
                {
                    Code = "InvalidPassword",
                    Description = "Kullanıcı adı veya şifre hatalı."
                });

                errors.Add(new IdentityError
                {
                    Code = "RemainingAttempts",
                    Description = remaining.ToString()
                });

                return new LoginResultDto
                {
                    Succeeded = false,
                    Errors = errors
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role))
            {
                errors.Add(new IdentityError { Code = "NoRole", Description = "Kullanıcıya rol atanmadı." });
                return new LoginResultDto { Succeeded = false, Errors = errors };
            }

            return new LoginResultDto
            {
                Succeeded = true,
                Role = role,
                Errors = errors
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public Task<bool> CreateRoleAsync(UserRoleDto userRoleDto)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> AssignRoleAsync(List<AssignRoleDto> assignRoleDto)
        {
            throw new NotImplementedException();
            //foreach(var item in assignRoleDto)
            //{
            //    if(item.RoleExist)
            //    {
            //        await _userManager.AddToRoleAsync()
            //    }
            //}
        }

        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<string> GeneratePasswordResetTokenByIdAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı");

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordByIdAsync(int userId, string token, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
