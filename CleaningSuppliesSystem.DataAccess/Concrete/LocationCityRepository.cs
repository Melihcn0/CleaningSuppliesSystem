using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class LocationCityRepository : GenericRepository<LocationCity>, ILocationCityRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public LocationCityRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task SoftDeleteAsync(LocationCity locationCity)
        {
            _context.LocationCitys.Update(locationCity);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(LocationCity locationCity)
        {
            _context.LocationCitys.Update(locationCity);
            await _context.SaveChangesAsync();
        }
        public async Task<List<LocationCity>> GetActiveLocationCitiesAsync()
        {
            return await _context.LocationCitys
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<LocationCity>> GetDeletedLocationCitiesAsync()
        {
            return await _context.LocationCitys
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<LocationCity>> GetLocationCityWithLocationDistrictAsync()
        {
            return await _context.LocationCitys
                .Where(city => !city.IsDeleted)
                .Select(city => new LocationCity
                {
                    CityId = city.CityId,
                    CityName = city.CityName,
                    Districts = city.Districts
                        .Where(d => !d.IsDeleted)
                        .ToList()
                })
                .ToListAsync();
        }
    }
}
