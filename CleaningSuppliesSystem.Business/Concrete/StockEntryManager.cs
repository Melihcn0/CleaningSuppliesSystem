using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class StockEntryManager : GenericManager<StockEntry> , IStockEntryService
    {
        private readonly IStockEntryRepository _stockEntryRepository;

        public StockEntryManager(IRepository<StockEntry> repository, IStockEntryRepository stockEntryRepository) : base(repository)
        {
            _stockEntryRepository = stockEntryRepository;
        }
        public async Task<List<StockEntry>> TGetStockEntryWithProductsandCategoriesAsync()
        {
            return await _stockEntryRepository.GetStockEntryWithProductsandCategoriesAsync();
        }
        public async Task<StockEntry> TGetByIdAsyncWithProductsandCategories(int id)
        {
            return await _stockEntryRepository.GetByIdAsyncWithProductsandCategories(id);
        }
    }
}
