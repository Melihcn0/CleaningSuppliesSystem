using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<List<Service>> GetAllAsync();
        Task<Service> GetByIdAsync(int id);
        Task CreateAsync(Service service);
        Task UpdateAsync(Service service);
        Task SoftDeleteAsync(Service service);
        Task UndoSoftDeleteAsync(Service service);
        Task<List<Service>> GetActiveServicesAsync();
        Task<List<Service>> GetDeletedServicesAsync();
        Task<bool> SetAsShownAsync(int id);
    }
}
