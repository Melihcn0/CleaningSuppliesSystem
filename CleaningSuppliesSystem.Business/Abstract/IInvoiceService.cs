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
        Task<Invoice> TGetByIdAsyncWithOrder(int id);
    }
}
