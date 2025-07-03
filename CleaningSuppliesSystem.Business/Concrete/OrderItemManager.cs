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
    public class OrderItemManager : GenericManager<OrderItem> , IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemManager(IRepository<OrderItem> repository, IOrderItemRepository orderItemRepository)
            : base(repository)
        {
            _orderItemRepository = orderItemRepository;
        }
        public Task<List<OrderItem>> TGetOrderItemWithProductandCategoriesAsync()
        {
            return _orderItemRepository.GetOrderItemWithProductandCategoriesAsync();
        }
        public Task<OrderItem> TGetByIdAsyncWithProductandCategories(int id)
        {
            return _orderItemRepository.GetByIdAsyncWithProductandCategories(id);
        }
    }
}

