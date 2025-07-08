using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IStockEntryRepository : IRepository<StockEntry>
    {
        Task<List<StockEntry>> GetStockEntryWithProductsandCategoriesAsync();
        Task<StockEntry> GetByIdAsyncWithProductsandCategories(int id);
        Task<StockEntry> GetByIdAsync(int id);
        Task CreateAsync(StockEntry stockEntry);
        Task UpdateAsync(StockEntry stockEntry);
        Task SoftDeleteAsync(StockEntry stockEntry);
        Task UndoSoftDeleteAsync(StockEntry stockEntry);
    }
}
