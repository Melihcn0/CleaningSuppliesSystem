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
    public class CustomerCorporateAddressRepository : GenericRepository<CustomerCorporateAddress>, ICustomerCorporateAddressRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public CustomerCorporateAddressRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> SetAsDefaultAsync(int addressId)
        {
            var addressToSet = await _context.CustomerCorporateAddresses.FirstOrDefaultAsync(x => x.Id == addressId);
            if (addressToSet == null)
                return false;

            var otherAddresses = await _context.CustomerCorporateAddresses
                .Where(x => x.AppUserId == addressToSet.AppUserId && x.Id != addressId && x.IsDefault)
                .ToListAsync();

            foreach (var addr in otherAddresses)
            {
                addr.IsDefault = false;
                _context.CustomerCorporateAddresses.Update(addr);
            }

            addressToSet.IsDefault = true;
            _context.CustomerCorporateAddresses.Update(addressToSet);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CustomerCorporateAddress>> GetAllByUserIdAsync(int userId)
        {
            return await _context.CustomerCorporateAddresses
                .Where(x => x.AppUserId == userId)
                .ToListAsync();
        }
    }
}
