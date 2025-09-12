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
        Task<List<Order>> GetActiveOrdersWithDetailsAsync();
        Task<Order> GetOrderByIdWithDetailsAsync(int id);
        Task<Order?> GetPendingOrderByUserIdAsync(int userId);
        Task AddToPendingOrderAsync(int userId, int productId, int quantity, decimal unitPrice, decimal? discountRate);
        Task<List<Order>> GetOrdersWithItemsAsync();
        Task<List<Order>> GetOrdersByUserIdWithDetailsAsync(int userId);
        Task<List<Order>> GetCompletedOrdersAsync();
        Task<List<Order>> GetCancelledOrdersAsync();
        Task<Order> UpdateStatusAsync(int orderId, string status);
        Task<(bool CanOrder, string? Message)> ValidateCustomerProfileAsync(int userId);

    }
}
