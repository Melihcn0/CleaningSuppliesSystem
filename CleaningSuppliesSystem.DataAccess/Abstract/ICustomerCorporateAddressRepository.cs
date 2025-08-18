using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ICustomerCorporateAddressRepository : IRepository<CustomerCorporateAddress>
    {
        Task<bool> SetAsDefaultAsync(int addressId);
        Task<List<CustomerCorporateAddress>> GetAllByUserIdAsync(int userId);
    }
}
