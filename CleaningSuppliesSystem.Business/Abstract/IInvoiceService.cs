using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IInvoiceService : IGenericService<Invoice>
    {
        Task<List<Invoice>> TGetInvoiceWithOrderAsync();
        Task<Invoice> TGetByIdAsyncWithOrder(int orderId);
        Task<byte[]> TGenerateInvoicePdfAsync(int orderId);
        Task<Invoice> TGetInvoiceByOrderIdAsync(int orderId);
        Task<Invoice> TCreateInvoiceAsync(int orderId);
        Task<List<Invoice>> TGetInvoicesByUserIdAsync(int userId);


    }
}
