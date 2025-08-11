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
    public class ServiceIconRepository : GenericRepository<ServiceIcon>, IServiceIconRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public ServiceIconRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ServiceIcon> GetByIdAsync(int id)
        {
            return await _context.ServiceIcons.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task CreateAsync(ServiceIcon icon)
        {
            await _context.ServiceIcons.AddAsync(icon);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(ServiceIcon icon)
        {
            _context.ServiceIcons.Update(icon);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ServiceIcon>> GetActiveServiceIconsAsync()
        {
            return await _context.ServiceIcons
                .Where(tc => !tc.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<ServiceIcon>> GetDeletedServiceIconsAsync()
        {
            return await _context.ServiceIcons
                .Where(tc => tc.IsDeleted)
                .ToListAsync();
        }
        public async Task SoftDeleteAsync(ServiceIcon icon)
        {
            _context.ServiceIcons.Update(icon);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteAsync(ServiceIcon icon)
        {
            _context.ServiceIcons.Update(icon);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ServiceIcon>> GetUnusedActiveIconsAsync()
        {
            var usedIconIds = await _context.Services
                .Where(s => s.ServiceIconId != null && !s.IsDeleted)
                .Select(s => s.ServiceIconId)
                .Distinct()
                .ToListAsync();

            return await _context.ServiceIcons
                .Where(icon => !usedIconIds.Contains(icon.Id) && icon.IsShown && !icon.IsDeleted)
                .ToListAsync();
        }
    }
}
