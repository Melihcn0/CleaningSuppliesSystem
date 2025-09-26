using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IPromoAlertRepository : IRepository<PromoAlert>
    {
        Task<bool> SetAsShownAsync(int id);
        Task<List<PromoAlert>> GetAllPromoAlertAsync();
        Task<PromoAlert> GetFirstPromoAlertAsync();
    }
}
