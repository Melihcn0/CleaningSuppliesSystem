using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ICustomerIndivivualAddressRepository : IRepository<CustomerIndivivualAddress>
    {
        Task<bool> SetAsDefaultAsync(int addressId);
        Task<List<CustomerIndivivualAddress>> GetAllByUserIdAsync(int userId);
    }
}