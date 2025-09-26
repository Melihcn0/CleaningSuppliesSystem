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
        Task<List<AppRole>> TGetAllRolesAsync();
        Task<AppRole> TGetRoleByIdAsync(int id);
        Task<IdentityResult> TCreateRoleAsync(AppRole role);
        Task<bool> TShouldShowCreateRoleButtonAsync();
    }
}
