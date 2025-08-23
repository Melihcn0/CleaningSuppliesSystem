using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ICustomerIndividualAddressRepository : IRepository<CustomerIndividualAddress>
    {
        Task<bool> SetAsDefaultAsync(int addressId);
        Task<List<CustomerIndividualAddress>> GetAllByUserIdAsync(int userId);
    }
}