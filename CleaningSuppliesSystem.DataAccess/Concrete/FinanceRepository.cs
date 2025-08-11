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
    public class FinanceRepository : GenericRepository<Finance>, IFinanceRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public FinanceRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Finance> GetByIdAsync(int id)
        {
            return await _context.Finances.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task CreateAsync(Finance finance)
        {
            await _context.Finances.AddAsync(finance);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Finance finance)
        {
            _context.Finances.Update(finance);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteAsync(Finance finance)
        {
            _context.Finances.Update(finance);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteAsync(Finance finance)
        {
            _context.Finances.Update(finance);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Finance>> GetActiveFinancesAsync()
        {
            return await _context.Finances
                .Where(tc => !tc.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Finance>> GetDeletedFinancesAsync()
        {
            return await _context.Finances
                .Where(tc => tc.IsDeleted)
                .ToListAsync();
        }
        public async Task SoftDeleteRangeAsync(List<int> ids)
        {
            var finances = await _context.Finances
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var finance in finances)
            {
                finance.IsDeleted = true;
            }

            _context.Finances.UpdateRange(finances);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteRangeAsync(List<int> ids)
        {
            var finances = await _context.Finances
                .Where(c => ids.Contains(c.Id) && c.IsDeleted)
                .ToListAsync();

            foreach (var finance in finances)
            {
                finance.IsDeleted = false;
            }

            _context.Finances.UpdateRange(finances);
            await _context.SaveChangesAsync();
        }

        public async Task PermanentDeleteRangeAsync(List<int> ids)
        {
            var finances = await _context.Finances
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            _context.Finances.RemoveRange(finances);
            await _context.SaveChangesAsync();
        }
    }
}
