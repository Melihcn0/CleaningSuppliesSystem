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
    public class SecondaryBannerRepository : GenericRepository<SecondaryBanner>, ISecondaryBannerRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public SecondaryBannerRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SecondaryBanner> GetByIdAsync(int id)
        {
            return await _context.SecondaryBanners.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<SecondaryBanner>> GetAllAsync()
        {
            return await _context.SecondaryBanners.ToListAsync();
        }
        public async Task CreateAsync(SecondaryBanner secondaryBanner)
        {
            await _context.SecondaryBanners.AddAsync(secondaryBanner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SecondaryBanner secondaryBanner)
        {
            _context.SecondaryBanners.Update(secondaryBanner);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(SecondaryBanner secondaryBanner)
        {
            _context.SecondaryBanners.Update(secondaryBanner);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(SecondaryBanner secondaryBanner)
        {
            _context.SecondaryBanners.Update(secondaryBanner);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SecondaryBanner>> GetActiveSecondaryBannersAsync()
        {
            return await _context.SecondaryBanners
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<SecondaryBanner>> GetDeletedSecondaryBannersAsync()
        {
            return await _context.SecondaryBanners
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }
        public async Task<bool> SetAsShownAsync(int id)
        {
            var secondaryBannerToSet = await _context.SecondaryBanners.FirstOrDefaultAsync(x => x.Id == id);
            if (secondaryBannerToSet == null || secondaryBannerToSet.IsDeleted)
                return false;

            var currentlyShown = await _context.SecondaryBanners
                .Where(x => x.IsShown && !x.IsDeleted && x.Id != id)
                .ToListAsync();

            foreach (var secondaryBanner in currentlyShown)
            {
                secondaryBanner.IsShown = false;
                _context.SecondaryBanners.Update(secondaryBanner);
            }

            secondaryBannerToSet.IsShown = true;
            _context.SecondaryBanners.Update(secondaryBannerToSet);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
