using AutoMapper;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IRoleService
    {
        Task<List<AppRole>> GetAllRolesAsync();
        Task<AppRole> GetRoleByIdAsync(int id);
        Task<IdentityResult> CreateRoleAsync(AppRole role);
        Task<bool> ShouldShowCreateRoleButtonAsync();
    }
}
