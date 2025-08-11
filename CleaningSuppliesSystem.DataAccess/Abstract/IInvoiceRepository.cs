using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<List<Invoice>> GetInvoiceWithOrderAsync();
        Task<Invoice?> GetByIdAsyncWithOrder(int id);
        Task<Invoice?> GetInvoiceByOrderIdAsync(int orderId);
        Task<List<Invoice>> GetInvoicesByUserIdAsync(int userId);
    }
}
