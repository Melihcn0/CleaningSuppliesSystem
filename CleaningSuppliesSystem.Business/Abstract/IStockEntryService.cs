using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IStockEntryService : IGenericService<StockEntry>
    {
        Task<List<StockEntry>> TGetStockEntryWithProductsandCategoriesAsync();
        Task<StockEntry> TGetByIdAsyncWithProductsandCategories(int id);
    }
}
