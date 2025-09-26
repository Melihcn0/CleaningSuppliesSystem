    using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
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
        Task<List<Order>> TGetActiveOrdersWithDetailsAsync();
        Task<Order> TGetOrderByIdWithDetailsAsync(int id);
        Task<Order?> TGetPendingOrderByUserIdAsync(int userId);
        Task TAddToPendingOrderAsync(int userId, int productId, int quantity, decimal unitPrice, decimal? discountRate);
        Task<List<Order>> TGetOrdersWithItemsAsync();
        Task<List<Order>> TGetOrdersByUserIdWithDetailsAsync(int userId);
        Task<List<Order>> TGetCompletedOrdersAsync();
        Task<List<Order>> TGetCancelledOrdersAsync();
        Task<OrderStatusUpdateDto> TUpdateStatusAsync(int orderId, string status);
        Task<(bool CanOrder, string? Message)> TValidateCustomerProfileAsync(int userId);
        Task<List<ExpiredOrderDto>> TGetExpiredOrdersAsync();
    }
}