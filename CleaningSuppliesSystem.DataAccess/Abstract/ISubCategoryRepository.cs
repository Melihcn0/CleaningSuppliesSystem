using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        Task<SubCategory> GetByIdAsync(int id);
        Task CreateAsync(SubCategory subCategory);
        Task UpdateAsync(SubCategory subCategory);
        Task SoftDeleteAsync(SubCategory subCategory);
        Task UndoSoftDeleteAsync(SubCategory subCategory);
        Task<List<SubCategory>> GetActiveByTopCategoryIdAsync(int topCategoryId);
        Task<List<SubCategory>> GetActiveSubCategoriesAsync();
        Task<List<SubCategory>> GetDeletedSubCategoriesAsync();
        Task SoftDeleteRangeAsync(List<int> ids);
        Task UndoSoftDeleteRangeAsync(List<int> ids);
        Task PermanentDeleteRangeAsync(List<int> ids);

    }
}
