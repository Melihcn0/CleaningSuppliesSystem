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
    public class TopCategoryRepository : GenericRepository<TopCategory>, ITopCategoryRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public TopCategoryRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<TopCategory> GetByIdAsync(int id)
        //{
        //    return await _context.TopCategories.FirstOrDefaultAsync(p => p.Id == id);
        //}

        //public async Task CreateAsync(TopCategory topCategory)
        //{
        //    await _context.TopCategories.AddAsync(topCategory);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(TopCategory topCategory)
        //{
        //    _context.TopCategories.Update(topCategory);
        //    await _context.SaveChangesAsync();
        //}

        public async Task SoftDeleteAsync(TopCategory topCategory)
        {
            _context.TopCategories.Update(topCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteAsync(TopCategory topCategory)
        {
            _context.TopCategories.Update(topCategory);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TopCategory>> GetActiveTopCategoriesAsync()
        {
            return await _context.TopCategories
                .Where(tc => !tc.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<TopCategory>> GetDeletedTopCategoriesAsync()
        {
            return await _context.TopCategories
                .Where(c => c.IsDeleted)
                .ToListAsync();
        }

        public async Task SoftDeleteRangeAsync(List<int> ids)
        {
            var topCategories = await _context.TopCategories
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var topCategory in topCategories)
            {
                topCategory.IsDeleted = true;
            }

            _context.TopCategories.UpdateRange(topCategories);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteRangeAsync(List<int> ids)
        {
            var topCategories = await _context.TopCategories
                .Where(c => ids.Contains(c.Id) && c.IsDeleted)
                .ToListAsync();

            foreach (var topCategory in topCategories)
            {
                topCategory.IsDeleted = false;
            }

            _context.TopCategories.UpdateRange(topCategories);
            await _context.SaveChangesAsync();
        }

        public async Task PermanentDeleteRangeAsync(List<int> ids)
        {
            var topCategories = await _context.TopCategories
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            _context.TopCategories.RemoveRange(topCategories);
            await _context.SaveChangesAsync();
        }
    }
}
