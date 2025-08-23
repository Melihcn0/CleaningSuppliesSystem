using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ILocationDistrictRepository : IRepository<LocationDistrict>
    {
        Task SoftDeleteAsync(LocationDistrict locationDistrict);
        Task UndoSoftDeleteAsync(LocationDistrict locationDistrict);
        Task<List<LocationDistrict>> GetActiveLocationDistrictsAsync();
        Task<List<LocationDistrict>> GetDeletedLocationDistrictsAsync();
        Task<List<LocationDistrict>> GetActiveByCityIdAsync(int cityId);
    }
}
