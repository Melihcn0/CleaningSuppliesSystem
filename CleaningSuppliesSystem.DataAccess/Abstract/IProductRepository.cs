using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetByIdAsync(int id);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task SoftDeleteAsync(Product product);
        Task UndoSoftDeleteAsync(Product product);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetActiveProductsAsync();
        Task<List<Product>> GetActiveByBrandsIdAsync(int brandId);
        Task<List<Product>> GetDeletedProductAsync();
        Task SoftDeleteRangeAsync(List<int> ids);
        Task UndoSoftDeleteRangeAsync(List<int> ids);
        Task PermanentDeleteRangeAsync(List<int> ids);
    }
}
