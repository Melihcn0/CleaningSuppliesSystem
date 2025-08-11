using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IServiceIconRepository : IRepository<ServiceIcon>
    {
        Task<ServiceIcon> GetByIdAsync(int id);
        Task CreateAsync(ServiceIcon serviceIcon);
        Task UpdateAsync(ServiceIcon serviceIcon);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(ServiceIcon serviceIcon);
        Task UndoSoftDeleteAsync(ServiceIcon serviceIcon);
        Task<List<ServiceIcon>> GetActiveServiceIconsAsync();
        Task<List<ServiceIcon>> GetDeletedServiceIconsAsync();
        Task<List<ServiceIcon>> GetUnusedActiveIconsAsync();
    }
}