using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface ITopCategoryRepository : IRepository<TopCategory>
    {
        //Task<TopCategory> GetByIdAsync(int id);
        //Task CreateAsync(TopCategory topCategory);
        //Task UpdateAsync(TopCategory topCategory);
        Task SoftDeleteAsync(TopCategory topCategory);
        Task UndoSoftDeleteAsync(TopCategory topCategory);
        Task<List<TopCategory>> GetActiveTopCategoriesAsync();
        Task<List<TopCategory>> GetDeletedTopCategoriesAsync();
        Task SoftDeleteRangeAsync(List<int> ids);
        Task UndoSoftDeleteRangeAsync(List<int> ids);
        Task PermanentDeleteRangeAsync(List<int> ids);
    }
}
