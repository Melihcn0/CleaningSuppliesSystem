using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class OrderRepository : GenericRepository<Order> , IOrderRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderRepository(CleaningSuppliesSystemContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Order>> GetActiveOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .AsNoTracking() // Sadece okuma amaçlı, EF tracking yapmaz
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
                .OrderByDescending(o => o.CreatedDate) // En yeni sipariş en üstte
                .ToListAsync();
        }
        public async Task<Order> GetOrderByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
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
            return await _context.Orders
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
            return await _context.Orders
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
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.AppUserId == userId && o.Status == "Onay Bekleniyor");
        }
        public async Task AddToPendingOrderAsync(int userId, int productId, int quantity, decimal unitPrice, decimal? discountRate)
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
                    OrderNumber = await GenerateUniqueOrderNumberAsync(),
                    OrderItems = new List<OrderItem>()
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            // unitPrice ve discountRate parametrelerden geliyor artık
            var discountedUnitPrice = discountRate.HasValue ? unitPrice * (1 - discountRate.Value / 100) : unitPrice;

            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);

            // 🔥 Stok kontrolü
            int existingQuantity = existingItem?.Quantity ?? 0;
            int remaining = (int)product.StockQuantity - existingQuantity;

            if (remaining <= 0)
            {
                throw new InvalidOperationException(
                    $"Sepetinizde <strong>{product.Name}</strong> ürününden {existingQuantity} adet var, en fazla {product.StockQuantity} adet alabilirsiniz."
                );
            }
            else if (quantity > remaining)
            {
                throw new InvalidOperationException(
                    $"Sepetinizde <strong>{product.Name}</strong> ürününden {existingQuantity} adet var, {remaining} {(remaining == 1 ? "adet" : "adet")} daha ekleyebilirsiniz."
                );
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UnitPrice = unitPrice;
                existingItem.DiscountRate = discountRate;
                existingItem.DiscountedUnitPrice = discountedUnitPrice;
                _context.OrderItems.Update(existingItem);
            }
            else
            {
                var newItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountRate = discountRate,
                    DiscountedUnitPrice = discountedUnitPrice
                };
                await _context.OrderItems.AddAsync(newItem);
            }

            await _context.SaveChangesAsync();
        }
        private async Task<string> GenerateUniqueOrderNumberAsync()
        {
            var lastOrder = await _context.Orders
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();

            int nextOrderId = (lastOrder?.Id ?? 0) + 1;

            string formattedNumber = nextOrderId.ToString("D6");
            string year = DateTime.Now.Year.ToString();

            return $"{year}-{formattedNumber}";
        }
        public async Task<List<Order>> GetOrdersWithItemsAsync()
        {
            return await _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
        public async Task<List<Order>> GetOrdersByUserIdWithDetailsAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.AppUserId == userId)
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Invoice)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }


        public async Task<Order> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.Id == orderId);
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
                    var invoiceRepo = new GenericRepository<Invoice>(_context);
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
                    var user = _httpContextAccessor.HttpContext.User;
                    if (user.IsInRole("Admin"))
                    {
                        order.OrderNote = "Yetkili tarafından iptal edilmiştir";
                    }
                    else if (user.IsInRole("Customer"))
                    {
                        order.OrderNote = "Müşteri tarafından iptal edilmiştir";
                    }
                    break;
            }

            _context.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // 🔹 Müşteri bilgi kontrolü
        public async Task<(bool CanOrder, string? Message)> ValidateCustomerProfileAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.CustomerIndividualAddresses)
                .Include(u => u.CustomerCorporateAddresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(user.PhoneNumber) || string.IsNullOrWhiteSpace(user.NationalId))
                return (false, "Sipariş verebilmek için lütfen Profil Ayarlarından Telefon Numaranızı ve T.C. Kimlik Numaranızı eksiksiz olarak doldurun.");

            bool hasDefaultIndividual = user.CustomerIndividualAddresses.Any(a => a.IsDefault);
            bool hasDefaultCorporate = user.CustomerCorporateAddresses.Any(a => a.IsDefault);

            if (!hasDefaultIndividual && !hasDefaultCorporate) 
                return (false, "Sipariş verebilmek için lütfen en az bir adres ekleyin ve bir adresi varsayılan olarak seçin.");


            return (true, null);
        }



    }
}
