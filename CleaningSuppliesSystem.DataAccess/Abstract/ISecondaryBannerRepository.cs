using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ISecondaryBannerRepository : IRepository<SecondaryBanner>
    {
        Task<SecondaryBanner> GetByIdAsync(int id);
        Task<List<SecondaryBanner>> GetAllAsync();
        Task CreateAsync(SecondaryBanner banner2);
        Task UpdateAsync(SecondaryBanner banner2);
        Task SoftDeleteAsync(SecondaryBanner banner2);
        Task UndoSoftDeleteAsync(SecondaryBanner banner2);
        Task<List<SecondaryBanner>> GetActiveSecondaryBannersAsync();
        Task<List<SecondaryBanner>> GetDeletedSecondaryBannersAsync();
        Task<bool> SetAsShownAsync(int id);
    }
}
