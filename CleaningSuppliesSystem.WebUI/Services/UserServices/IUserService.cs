using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace CleaningSuppliesSystem.WebUI.Services.UserServices
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(UserRegisterDto userRegisterDto);
        Task<LoginResultDto> LoginUserAsync(UserLoginDto userLoginDto);
        Task LogoutAsync();
        Task<bool> CreateRoleAsync(UserRoleDto userRoleDto);
        Task<bool> AssignRoleAsync(List<AssignRoleDto> assignRoleDto);
        Task<List<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<string> GeneratePasswordResetTokenByIdAsync(int userId);
        Task<IdentityResult> ResetPasswordByIdAsync(int userId, string token, string newPassword);
        Task<AppUser?> GetUserByEmailAsync(string email);
    }
}
