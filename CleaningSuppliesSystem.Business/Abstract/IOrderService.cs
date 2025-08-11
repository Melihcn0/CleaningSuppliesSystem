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
        Task<List<Order>> TGetOrderItemWithAppUserandOrderItemsandInvoiceAsync();
        Task<Order> TGetByIdAsyncWithAppUserandOrderItemsandInvoice(int id);
        Task<Order?> TGetPendingOrderByUserIdAsync(int userId);
        Task TAddToPendingOrderAsync(int userId, int productId, int quantity);
        Task<List<ResultOrderDto>> TGetOrdersWithItemsAsync();
        Task<List<ResultOrderDto>> TGetOrdersWithItemsByUserIdAsync(int userId);
        Task<List<Order>> TGetCompletedAndCancelledOrdersAsync();
        Task<OrderStatusUpdateDto> UpdateStatusAsync(int orderId, string status);

    }
}
