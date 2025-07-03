using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class InvoiceManager : GenericManager<Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _ınvoiceRepository;

        public InvoiceManager(IRepository<Invoice> repository, IInvoiceRepository ınvoiceRepository)
            : base(repository)
        {
            _ınvoiceRepository = ınvoiceRepository;
        }
        public async Task<List<Invoice>> TGetInvoiceWithOrderAsync()
        {
            return await _ınvoiceRepository.GetInvoiceWithOrderAsync();
        }
        public async Task<Invoice> TGetByIdAsyncWithOrder(int id)
        {
            return await _ınvoiceRepository.GetByIdAsyncWithOrder(id);
        }
    }
}