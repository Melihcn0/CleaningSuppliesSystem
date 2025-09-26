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
        public async Task TDecrementQuantityAsync(int id)
        {
            var item = await _orderItemRepository.GetByIdAsync(id);
            if (item == null) throw new Exception("Ürün bulunamadı");

            if (item.Quantity > 1)
            {
                item.Quantity--;
                await _orderItemRepository.UpdateAsync(item);
            }
            else
            {
                await _orderItemRepository.DeleteAsync(id);
            }
        }

        public async Task TIncrementQuantityAsync(int id)
        {
            var item = await _orderItemRepository.GetByIdAsync(id);
            if (item == null)
                throw new Exception("Ürün bulunamadı");

            item.Quantity++;
            await _orderItemRepository.UpdateAsync(item);
        }

    }
}

