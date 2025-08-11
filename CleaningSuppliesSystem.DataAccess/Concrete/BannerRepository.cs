using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class BannerRepository : GenericRepository<Banner>, IBannerRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public BannerRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Banner> GetByIdAsync(int id)
        {
            return await _context.Banners.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<Banner>> GetAllAsync()
        {
            return await _context.Banners.ToListAsync();
        }
        public async Task CreateAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Banner>> GetActiveBannersAsync()
        {
            return await _context.Banners
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Banner>> GetDeletedBannersAsync()
        {
            return await _context.Banners
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Banner>> GetActiveBannerAsync()
        {
            return await _context.Banners
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Banner>> GetDeletedBannerAsync()
        {
            return await _context.Banners
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }
        public async Task<bool> SetAsShownAsync(int id)
        {
            var bannerToSet = await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);
            if (bannerToSet == null || bannerToSet.IsDeleted)
                return false;

            var currentlyShown = await _context.Banners
                .Where(x => x.IsShown && !x.IsDeleted && x.Id != id)
                .ToListAsync();

            foreach (var banner in currentlyShown)
            {
                banner.IsShown = false;
                _context.Banners.Update(banner);
            }

            bannerToSet.IsShown = true;
            _context.Banners.Update(bannerToSet);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
