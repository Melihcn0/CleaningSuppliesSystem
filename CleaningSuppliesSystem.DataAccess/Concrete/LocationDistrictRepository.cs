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
    public class LocationDistrictRepository : GenericRepository<LocationDistrict>, ILocationDistrictRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public LocationDistrictRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task SoftDeleteAsync(LocationDistrict locationDistrict)
        {
            _context.LocationDistricts.Update(locationDistrict);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(LocationDistrict locationDistrict)
        {
            _context.LocationDistricts.Update(locationDistrict);
            await _context.SaveChangesAsync();
        }
        public async Task<List<LocationDistrict>> GetActiveLocationDistrictsAsync()
        {
            return await _context.LocationDistricts
                .Include(b => b.City)
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<LocationDistrict>> GetDeletedLocationDistrictsAsync()
        {
            return await _context.LocationDistricts
                .Include(b => b.City)
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<LocationDistrict>> GetActiveByCityIdAsync(int cityId)
        {
            return await _context.LocationDistricts
                .Where(d => d.CityId == cityId && !d.IsDeleted)
                .ToListAsync();
        }
    }
}
