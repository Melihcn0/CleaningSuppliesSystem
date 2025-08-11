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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public CategoryRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.TopCategory)
                .Include(c => c.SubCategory)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Category>> GetDeletedCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.TopCategory)
                .Include(c => c.SubCategory)
                .Where(c => c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Category>> GetActiveBySubCategoryIdAsync(int subCategoryId)
        {
            return await _context.Categories
                .Where(sc => sc.SubCategoryId == subCategoryId && !sc.IsDeleted)
                .ToListAsync();
        }

        public async Task SoftDeleteRangeAsync(List<int> ids)
        {
            var categories = await _context.Categories
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.IsDeleted = true;
            }

            _context.Categories.UpdateRange(categories);
            await _context.SaveChangesAsync();
        }

        public async Task UndoSoftDeleteRangeAsync(List<int> ids)
        {
            var categories = await _context.Categories
                .Where(c => ids.Contains(c.Id) && c.IsDeleted)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.IsDeleted = false;
            }

            _context.Categories.UpdateRange(categories);
            await _context.SaveChangesAsync();
        }

        public async Task PermanentDeleteRangeAsync(List<int> ids)
        {
            var categories = await _context.Categories
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            _context.Categories.RemoveRange(categories);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Category>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
        }



    }
}
