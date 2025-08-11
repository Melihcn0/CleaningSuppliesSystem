using CleaningSuppliesSystem.DTO.DTOs.UserDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
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
        Task<bool> AssignRoleToUserAsync(int userId, string selectedRole);
        Task<bool> ToggleUserStatusAsync(int userId, bool newStatus);
        Task<List<UserListDto>> GetAllUsersWithRolesAsync();
        Task<List<AssignRoleDto>> GetUserRolesForAssignAsync(int userId);



    }
}
