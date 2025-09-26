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
        Task<IdentityResult> TCreateUserAsync(UserRegisterDto userRegisterDto);
        Task<LoginResultDto> TLoginUserAsync(UserLoginDto userLoginDto);
        Task TLogoutAsync();
        Task<bool> TCreateRoleAsync(UserRoleDto userRoleDto);
        Task<bool> TAssignRoleAsync(List<AssignRoleDto> assignRoleDto);
        Task<List<AppUser>> TGetAllUsersAsync();
        Task<AppUser> TGetUserByIdAsync(int id);
        Task<string> TGeneratePasswordResetTokenByIdAsync(int userId);
        Task<IdentityResult> TResetPasswordByIdAsync(int userId, string token, string newPassword);
        Task<AppUser?> TGetUserByEmailAsync(string email);
        Task<bool> TAssignRoleToUserAsync(int userId, string selectedRole);
        Task<bool> TToggleUserStatusAsync(int userId, bool newStatus);
        Task<List<UserListDto>> TGetAllUsersWithRolesAsync();
        Task<List<UserListDto>> TGetNonDeveloperUsersWithRolesAsync();
        Task<List<AssignRoleDto>> TGetUserRolesForAssignAsync(int userId);



    }
}
