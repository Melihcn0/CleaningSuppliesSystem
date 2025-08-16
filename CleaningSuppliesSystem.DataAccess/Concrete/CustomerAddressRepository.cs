using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class CustomerAddressRepository : GenericRepository<CustomerAddress>, ICustomerAddressRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public CustomerAddressRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
    }
}
