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
        Task<List<Product>> GetProductsWithCategoriesAsync();
        Task<Product> GetByIdAsyncWithCategory(int id);
    }
}
