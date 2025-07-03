using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrderItemWithAppUserandOrderItemsandInvoiceAsync();
        Task<Order> GetByIdAsyncWithAppUserandOrderItemsandInvoice(int id);
    }
}
