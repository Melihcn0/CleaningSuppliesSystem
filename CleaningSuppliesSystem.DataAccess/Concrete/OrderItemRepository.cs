using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        private readonly CleaningSuppliesSystemContext _cleaningSuppliesContext;
        public OrderItemRepository(CleaningSuppliesSystemContext _context) : base(_context)
        {
            _cleaningSuppliesContext = _context;
        }
        public async Task<List<OrderItem>> GetOrderItemWithProductandCategoriesAsync()
        {
            return await _cleaningSuppliesContext.OrderItems.Include(oi => oi.Product).ThenInclude(oi => oi.Category).ToListAsync();
        }

        public async Task<OrderItem> GetByIdAsyncWithProductandCategories(int id)
        {
            return await _cleaningSuppliesContext.OrderItems.Include(oi => oi.Product).ThenInclude(oi => oi.Category).FirstOrDefaultAsync(oi => oi.Id == id);
        }
    }
}
