using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IStockRepository : IRepository<Product>
    {
        Task<bool> AssignStockAsync(int productId, int quantity, bool isStockIn);
        Task<List<Product>> GetActiveProductsAsync();

    }
}
