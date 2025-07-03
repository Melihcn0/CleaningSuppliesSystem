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
    public class OrderRepository : GenericRepository<Order> , IOrderRepository
    {
        private readonly CleaningSuppliesSystemContext _cleaningSuppliesContext;
        public OrderRepository(CleaningSuppliesSystemContext _context) : base(_context)
        {
            _cleaningSuppliesContext = _context;
        }

        public async Task<List<Order>> GetOrderItemWithAppUserandOrderItemsandInvoiceAsync()
        {
            return await _cleaningSuppliesContext.Orders.Include(o => o.AppUser).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category).Include(o => o.Invoice).ToListAsync();

        }

        public async Task<Order> GetByIdAsyncWithAppUserandOrderItemsandInvoice(int id)
        {
            return await _cleaningSuppliesContext.Orders.Include(o => o.AppUser).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category).Include(o => o.Invoice).FirstOrDefaultAsync(oi => oi.Id == id);
        }
    }
}
