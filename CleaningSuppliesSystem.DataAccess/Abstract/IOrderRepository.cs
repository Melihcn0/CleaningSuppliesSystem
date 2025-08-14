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
        Task<Order?> GetPendingOrderByUserIdAsync(int userId);
        Task AddToPendingOrderAsync(int userId, int productId, int quantity);
        Task<List<Order>> GetOrdersWithItemsAsync();
        Task<List<Order>> GetOrdersWithItemsByUserIdAsync(int userId);
        Task<List<Order>> GetCompletedOrdersAsync();
        Task<List<Order>> GetCancelledOrdersAsync();
        Task<Order> UpdateStatusAsync(int orderId, string status);



    }
}
