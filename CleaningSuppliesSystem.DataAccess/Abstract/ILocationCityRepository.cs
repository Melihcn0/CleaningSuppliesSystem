using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ILocationCityRepository : IRepository<LocationCity>
    {
        Task SoftDeleteAsync(LocationCity locationCity);
        Task UndoSoftDeleteAsync(LocationCity locationCity);
        Task<List<LocationCity>> GetActiveLocationCitiesAsync();
        Task<List<LocationCity>> GetDeletedLocationCitiesAsync();
        Task<List<LocationCity>> GetLocationCityWithLocationDistrictAsync();
    }
}
