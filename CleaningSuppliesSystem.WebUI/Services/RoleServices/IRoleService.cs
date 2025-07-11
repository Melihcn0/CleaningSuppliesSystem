using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using Microsoft.AspNetCore.Identity;

namespace CleaningSuppliesSystem.WebUI.Services.RoleServices
{
    public interface IRoleService
    {
        Task<List<ResultRoleDto>> GetAllRolesAsync();
        Task CreateRoleAsync(CreateRoleDto createRoleDto);
        Task DeleteRoleAsync(int id);
        Task<bool> ShouldShowCreateRoleButtonAsync();

        Task<List<string>> GetMissingRolesAsync();
    }
}
