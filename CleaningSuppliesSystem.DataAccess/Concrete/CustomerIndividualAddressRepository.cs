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
    public class CustomerIndivivualAddressRepository : GenericRepository<CustomerIndivivualAddress>, ICustomerIndivivualAddressRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public CustomerIndivivualAddressRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> SetAsDefaultAsync(int addressId)
        {
            var addressToSet = await _context.CustomerIndivivualAddresses.FirstOrDefaultAsync(x => x.Id == addressId);
            if (addressToSet == null)
                return false;

            var otherAddresses = await _context.CustomerIndivivualAddresses
                .Where(x => x.AppUserId == addressToSet.AppUserId && x.Id != addressId && x.IsDefault)
                .ToListAsync();

            foreach (var addr in otherAddresses)
            {
                addr.IsDefault = false;
                _context.CustomerIndivivualAddresses.Update(addr);
            }

            addressToSet.IsDefault = true;
            _context.CustomerIndivivualAddresses.Update(addressToSet);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CustomerIndivivualAddress>> GetAllByUserIdAsync(int userId)
        {
            return await _context.CustomerIndivivualAddresses
                .Where(x => x.AppUserId == userId)
                .ToListAsync();
        }

    }
}
