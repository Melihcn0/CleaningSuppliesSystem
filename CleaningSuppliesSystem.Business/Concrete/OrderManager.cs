using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class OrderManager : GenericManager<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderManager(IRepository<Order> repository, IOrderRepository orderRepository)
            : base(repository)
        {
            _orderRepository = orderRepository;
        }
        public Task<List<Order>> TGetOrderItemWithAppUserandOrderItemsandInvoiceAsync()
        {
            return _orderRepository.GetOrderItemWithAppUserandOrderItemsandInvoiceAsync();
        }

        public Task<Order> TGetByIdAsyncWithAppUserandOrderItemsandInvoice(int id)
        {
            return _orderRepository.GetByIdAsyncWithAppUserandOrderItemsandInvoice(id);
        }
    }
}
