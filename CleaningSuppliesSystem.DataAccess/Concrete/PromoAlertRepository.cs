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
    public class PromoAlertRepository : GenericRepository<PromoAlert>, IPromoAlertRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public PromoAlertRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> SetAsShownAsync(int id)
        {
            var promoAlertToSet = await _context.PromoAlerts.FirstOrDefaultAsync(x => x.Id == id);
            if (promoAlertToSet == null)
                return false;

            var currentlyShown = await _context.PromoAlerts
                .Where(x => x.IsShown && x.Id != id)
                .ToListAsync();

            foreach (var promoAlert in currentlyShown)
            {
                promoAlert.IsShown = false;
                _context.PromoAlerts.Update(promoAlert);
            }

            promoAlertToSet.IsShown = true;
            _context.PromoAlerts.Update(promoAlertToSet);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<PromoAlert>> GetAllPromoAlertAsync()
        {
            return await _context.PromoAlerts.ToListAsync();
        }
        public async Task<PromoAlert> GetFirstPromoAlertAsync()
        {
            return await _context.PromoAlerts
                .Where(x => x.IsShown)
                .FirstOrDefaultAsync();
        }

    }
}
