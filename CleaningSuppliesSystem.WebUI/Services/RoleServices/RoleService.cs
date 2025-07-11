using AutoMapper;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.WebUI.Services.RoleServices
{
    public class RoleService(RoleManager<AppRole> _roleManager, IMapper _mapper) : IRoleService
    {
        public async Task CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            var role = _mapper.Map<AppRole>(createRoleDto);
            await _roleManager.CreateAsync(role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            var value = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);
            await _roleManager.DeleteAsync(value);
        }

        public async Task<List<ResultRoleDto>> GetAllRolesAsync()
        {
            var values = await _roleManager.Roles.ToListAsync();
            return _mapper.Map<List<ResultRoleDto>>(values);
        }

        public async Task<bool> ShouldShowCreateRoleButtonAsync()
        {
            var roleNames = new[] { "Admin", "Customer" };

            var existingRoles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();

            return roleNames.Any(role => !existingRoles.Contains(role));
        }
        public async Task<List<string>> GetMissingRolesAsync()
        {
            var predefinedRoles = new List<string> { "Admin", "Customer" };

            var existingRoles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();

            var missingRoles = predefinedRoles.Except(existingRoles).ToList();
            return missingRoles;
        }
    }
}
