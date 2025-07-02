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
        private readonly CleaningSuppliesSystemContext _cleaningSuppliesContext;
        public StockEntryRepository(CleaningSuppliesSystemContext _context) : base(_context)
        {
            _cleaningSuppliesContext = _context;
        }
        public async Task<List<StockEntry>> GetStockEntryWithProductsandCategoriesAsync()
        {
            return await _cleaningSuppliesContext.StockEntries.Include(p => p.Product).ThenInclude(p => p.Category).ToListAsync();
        }
        public async Task<StockEntry> GetByIdAsyncWithProductsandCategories(int id)
        {
            return await _cleaningSuppliesContext.StockEntries.Include(p => p.Product).ThenInclude(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
