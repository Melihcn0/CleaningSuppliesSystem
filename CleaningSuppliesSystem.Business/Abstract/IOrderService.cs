using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IOrderService : IGenericService<Order>
    {
        Task<List<Order>> TGetOrderItemWithAppUserandOrderItemsandInvoiceAsync();
        Task<Order> TGetByIdAsyncWithAppUserandOrderItemsandInvoice(int id);
    }
}
