using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public BrandRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Brand>> GetActiveBrandsAsync()
        {
            return await _context.Brands
                .Include(b => b.Category)
                    .ThenInclude(c => c.SubCategory)
                        .ThenInclude(sc => sc.TopCategory)
                .Where(b => !b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Brand>> GetDeletedBrandsAsync()
        {
            return await _context.Brands
                .Include(b => b.Category)
                    .ThenInclude(c => c.SubCategory)
                        .ThenInclude(sc => sc.TopCategory)
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Brand>> GetActiveByCategoryIdAsync(int categoryId)
        {
            return await _context.Brands
                .Where(sc => sc.CategoryId == categoryId && !sc.IsDeleted)
                .ToListAsync();
        }
        public async Task SoftDeleteRangeAsync(List<int> ids)
        {
            var brands = await _context.Brands
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var brand in brands)
            {
                brand.IsDeleted = true;
            }

            _context.Brands.UpdateRange(brands);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteRangeAsync(List<int> ids)
        {
            var brands = await _context.Brands
                .Where(c => ids.Contains(c.Id) && c.IsDeleted)
                .ToListAsync();

            foreach (var brand in brands)
            {
                brand.IsDeleted = false;
            }

            _context.Brands.UpdateRange(brands);
            await _context.SaveChangesAsync();
        }
        public async Task PermanentDeleteRangeAsync(List<int> ids)
        {
            var brands = await _context.Brands
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            _context.Brands.RemoveRange(brands);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> AnyIncludeSoftDeletedAsync(Expression<Func<Brand, bool>> predicate)
        {
            return await _context.Brands
                .IgnoreQueryFilters()
                .AnyAsync(predicate);
        }
        public async Task<List<Brand>> GetAllIncludeSoftDeletedAsync(Expression<Func<Brand, bool>> predicate)
        {
            return await _context.Brands
                .IgnoreQueryFilters()
                .Where(predicate)
                .ToListAsync();
        }

    }
}
