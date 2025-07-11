﻿using CleaningSuppliesSystem.DataAccess.Abstract;
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
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly CleaningSuppliesSystemContext _cleaningSuppliesContext;
        public InvoiceRepository(CleaningSuppliesSystemContext _context) : base(_context)
        {
            _cleaningSuppliesContext = _context;
        }
        public async Task<List<Invoice>> GetInvoiceWithOrderAsync()
        {
            return await _cleaningSuppliesContext.Invoices.Include(x => x.Order).ToListAsync();
        }
        public async Task<Invoice> GetByIdAsyncWithOrder(int id)
        {
            return await _cleaningSuppliesContext.Invoices.Include(x => x.Order).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
