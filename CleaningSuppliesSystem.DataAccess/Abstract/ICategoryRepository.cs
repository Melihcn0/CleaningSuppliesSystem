using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetByIdAsync(int id);
        Task CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task SoftDeleteAsync(Category category);
        Task UndoSoftDeleteAsync(Category category);
        Task<List<Category>> GetActiveBySubCategoryIdAsync(int subCategoryId);
        Task<List<Category>> GetActiveCategoriesAsync();
        Task<List<Category>> GetDeletedCategoriesAsync();
        Task SoftDeleteRangeAsync(List<int> ids);
        Task UndoSoftDeleteRangeAsync(List<int> ids);
        Task PermanentDeleteRangeAsync(List<int> ids);
        Task<List<Category>> GetByIdsAsync(List<int> ids);

    }
}
