using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IProductService : IGenericService<Product>
    {
        Task<List<Product>> TGetProductsWithCategoriesAsync();
        Task<Product> TGetByIdAsyncWithCategory(int id);
    }
}
