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
    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public SubCategoryRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SubCategory> GetByIdAsync(int id)
        {
            return await _context.SubCategories.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateAsync(SubCategory subCategories)
        {
            await _context.SubCategories.AddAsync(subCategories);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SubCategory subCategories)
        {
            _context.SubCategories.Update(subCategories);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(SubCategory subCategories)
        {
            _context.SubCategories.Update(subCategories);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(SubCategory subCategories)
        {
            _context.SubCategories.Update(subCategories);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SubCategory>> GetActiveByTopCategoryIdAsync(int topCategoryId)
        {
            return await _context.SubCategories
                .Where(sc => sc.TopCategoryId == topCategoryId && !sc.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<SubCategory>> GetActiveSubCategoriesAsync()
        {
            return await _context.SubCategories
                .Include(c => c.TopCategory)
                .Where(tc => !tc.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<SubCategory>> GetDeletedSubCategoriesAsync()
        {
            return await _context.SubCategories
                .Include(c => c.TopCategory)
                .Where(c => c.IsDeleted)
                .ToListAsync();
        }
        public async Task SoftDeleteRangeAsync(List<int> ids)
        {
            var subCategories = await _context.SubCategories
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var subCategory in subCategories)
            {
                subCategory.IsDeleted = true;
            }

            _context.SubCategories.UpdateRange(subCategories);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteRangeAsync(List<int> ids)
        {
            var subCategories = await _context.SubCategories
                .Where(c => ids.Contains(c.Id) && c.IsDeleted)
                .ToListAsync();

            foreach (var subCategory in subCategories)
            {
                subCategory.IsDeleted = false;
            }

            _context.SubCategories.UpdateRange(subCategories);
            await _context.SaveChangesAsync();
        }

        public async Task PermanentDeleteRangeAsync(List<int> ids)
        {
            var subCategories = await _context.SubCategories
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            _context.SubCategories.RemoveRange(subCategories);
            await _context.SaveChangesAsync();
        }
    }
}
