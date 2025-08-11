using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IBannerRepository : IRepository<Banner>
    {
        Task<Banner> GetByIdAsync(int id);
        Task<List<Banner>> GetAllAsync();
        Task CreateAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task SoftDeleteAsync(Banner banner);
        Task UndoSoftDeleteAsync(Banner banner);
        Task<List<Banner>> GetActiveBannersAsync();
        Task<List<Banner>> GetDeletedBannersAsync();
        Task<bool> SetAsShownAsync(int id);
    }
}
