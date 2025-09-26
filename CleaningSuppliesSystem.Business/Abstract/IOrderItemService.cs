using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IOrderItemService : IGenericService<OrderItem>
    {
        Task<List<OrderItem>> TGetOrderItemWithProductandCategoriesAsync();
        Task<OrderItem> TGetByIdAsyncWithProductandCategories(int id);
        Task TDecrementQuantityAsync(int id);
        Task TIncrementQuantityAsync(int id);
    }
}
