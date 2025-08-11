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
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public ServiceRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceIcon)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }
        public async Task CreateAsync(Service service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Service>> GetActiveServicesAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceIcon)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Service>> GetDeletedServicesAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceIcon)
                .Where(s => s.IsDeleted)
                .ToListAsync();
        }

        public async Task SoftDeleteAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> SetAsShownAsync(int id)
        {
            var serviceToSet = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceToSet == null || serviceToSet.IsDeleted)
                return false;

            var currentlyShown = await _context.Services
                .Where(x => x.IsShown && !x.IsDeleted && x.Id != id)
                .ToListAsync();

            foreach (var service in currentlyShown)
            {
                service.IsShown = false;
                _context.Services.Update(service);
            }

            serviceToSet.IsShown = true;
            _context.Services.Update(serviceToSet);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
