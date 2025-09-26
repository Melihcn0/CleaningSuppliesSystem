using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class RoleManager : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleManager(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<AppRole>> TGetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<AppRole> TGetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<IdentityResult> TCreateRoleAsync(AppRole role)
        {
            return await _roleRepository.CreateAsync(role);
        }

        public async Task<bool> TShouldShowCreateRoleButtonAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Count < 4;
        }
    }
}
