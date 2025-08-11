using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IRoleRepository
    {
        Task<List<AppRole>> GetAllAsync();
        Task<AppRole> GetByIdAsync(int id);
        Task<IdentityResult> CreateAsync(AppRole role);
    }
}
