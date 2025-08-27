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

            public async Task<List<Order>> GetActiveOrdersWithDetailsAsync()
            {
                return await _cleaningSuppliesContext.Orders
                    .Where(o => o.Status != "Teslim Edildi" && o.Status != "İptal Edildi")
                    .Include(o => o.AppUser)
                        .ThenInclude(u => u.CustomerIndividualAddresses) // bireysel
                    .Include(o => o.AppUser)
                        .ThenInclude(u => u.CustomerCorporateAddresses) // kurumsal
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Brand)
                                .ThenInclude(b => b.Category)
                                    .ThenInclude(c => c.SubCategory)
                                        .ThenInclude(sc => sc.TopCategory)
                    .Include(o => o.Invoice)
                    .OrderByDescending(o => o.CreatedDate)
                    .ToListAsync();
            }



        public async Task<Order> GetOrderByIdWithDetailsAsync(int id)
        {
            return await _cleaningSuppliesContext.Orders
                .Include(o => o.AppUser)
                    .ThenInclude(u => u.CustomerIndividualAddresses)
                .Include(o => o.AppUser)
                    .ThenInclude(u => u.CustomerCorporateAddresses)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Brand)
                            .ThenInclude(b => b.Category)
                                .ThenInclude(c => c.SubCategory)
                                    .ThenInclude(sc => sc.TopCategory)
                .Include(o => o.Invoice)
                    .ThenInclude(i => i.InvoiceItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }



        public async Task<List<Order>> GetCompletedOrdersAsync()
        {
            return await _cleaningSuppliesContext.Orders
                .Where(o => o.Status == "Teslim Edildi")
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Brand)
                            .ThenInclude(b => b.Category)
                                .ThenInclude(c => c.SubCategory)
                                    .ThenInclude(sc => sc.TopCategory)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetCancelledOrdersAsync()
        {
            return await _cleaningSuppliesContext.Orders
                .Where(o => o.Status == "İptal Edildi")
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Brand)
                            .ThenInclude(b => b.Category)
                                .ThenInclude(c => c.SubCategory)
                                    .ThenInclude(sc => sc.TopCategory)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }


        public async Task<Order?> GetPendingOrderByUserIdAsync(int userId)
        {
            return await _cleaningSuppliesContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.AppUserId == userId && o.Status == "Onay Bekleniyor");
        }
        public async Task AddToPendingOrderAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Miktar 0'dan küçük olamaz.");

            var order = await GetPendingOrderByUserIdAsync(userId);

            if (order == null)
            {
                order = new Order
                {
                    AppUserId = userId,
                    Status = "Onay Bekleniyor",
                    CreatedDate = DateTime.Now,
                    OrderNumber = GenerateOrderNumber(userId),
                    OrderItems = new List<OrderItem>()
                };

                await _cleaningSuppliesContext.Orders.AddAsync(order);
                await _cleaningSuppliesContext.SaveChangesAsync();

                order = await _cleaningSuppliesContext.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);
            }

            var product = await _cleaningSuppliesContext.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            var unitPrice = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.UnitPrice;

            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _cleaningSuppliesContext.OrderItems.Update(existingItem);
            }
            else
            {
                var newItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };
                await _cleaningSuppliesContext.OrderItems.AddAsync(newItem);
            }

            await _cleaningSuppliesContext.SaveChangesAsync();
        }



        private string GenerateOrderNumber(int orderId)
        {
            return $"INV-{DateTime.Now.Year}-{orderId}";
        }

        public async Task<List<Order>> GetOrdersWithItemsAsync()
        {
            return await _cleaningSuppliesContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdWithDetailsAsync(int userId)
        {
            return await _cleaningSuppliesContext.Orders
                .Where(o => o.AppUserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _cleaningSuppliesContext.Set<Order>().FindAsync(orderId);
            if (order == null) return null;

            order.Status = status;
            order.UpdatedDate = DateTime.Now;
            switch (status)
            {
                case "Onay Bekleniyor":
                    order.ApprovedDate = null;
                    break;
                case "Onaylandı":
                    order.ApprovedDate = DateTime.Now;
                    order.PreparingDate = null;
                    break;
                case "Hazırlanıyor":
                    order.PreparingDate = DateTime.Now;
                    order.ShippedDate = null;
                    var invoiceRepo = new GenericRepository<Invoice>(_cleaningSuppliesContext);
                    var invoice = await invoiceRepo.GetByFilterAsync(i => i.OrderId == order.Id);
                    if (invoice != null)
                    {
                        await invoiceRepo.DeleteAsync(invoice.Id);
                    }
                    break;
                case "Kargoya Verildi":
                    order.ShippedDate = DateTime.Now;
                    order.DeliveredDate = null;
                    break;
                case "Teslim Edildi":
                    order.DeliveredDate = DateTime.Now;
                    order.CanceledDate = null;
                    break;
                case "İptal Edildi":
                    order.CanceledDate = DateTime.Now;
                    order.ApprovedDate = null;
                    order.PreparingDate = null;
                    order.ShippedDate = null;
                    order.DeliveredDate = null;
                    break;
            }

            _cleaningSuppliesContext.Update(order);
            await _cleaningSuppliesContext.SaveChangesAsync();
            return order;
        }





    }
}
