using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ICompanyBankRepository : IRepository<CompanyBank>
    {
        Task<AppUser?> GetUserWithCompanyBankAsync(int userId);
        Task<CompanyBank?> GetFirstCompanyBankAsync();
    }
}
