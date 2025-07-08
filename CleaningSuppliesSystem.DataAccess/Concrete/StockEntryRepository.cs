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
    public class StockEntryRepository : GenericRepository<StockEntry> , IStockEntryRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public StockEntryRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<StockEntry>> GetStockEntryWithProductsandCategoriesAsync()
        {
            return await _context.StockEntries.Include(p => p.Product).ThenInclude(p => p.Category).ToListAsync();
        }
        public async Task<StockEntry> GetByIdAsyncWithProductsandCategories(int id)
        {
            return await _context.StockEntries.Include(p => p.Product).ThenInclude(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task CreateAsync(StockEntry stockEntry)
        {
            await _context.StockEntries.AddAsync(stockEntry);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(StockEntry stockEntry)
        {
            _context.StockEntries.Update(stockEntry);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteAsync(StockEntry stockEntry)
        {
            stockEntry.IsDeleted = true;
            _context.StockEntries.Update(stockEntry);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteAsync(StockEntry stockEntry)
        {
            stockEntry.IsDeleted = false;
            _context.StockEntries.Update(stockEntry);
            await _context.SaveChangesAsync();
        }
    }
}
