using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IBrandRepository : IRepository<Brand>
    {
        Task<Brand> GetByIdAsync(int id);
        Task CreateAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task SoftDeleteAsync(Brand brand);
        Task UndoSoftDeleteAsync(Brand brand);
        Task<List<Brand>> GetActiveByCategoryIdAsync(int categoryId);
        Task<List<Brand>> GetActiveBrandsAsync();
        Task<List<Brand>> GetDeletedBrandsAsync();
        Task SoftDeleteRangeAsync(List<int> ids);
        Task UndoSoftDeleteRangeAsync(List<int> ids);
        Task PermanentDeleteRangeAsync(List<int> ids);
        Task<bool> AnyIncludeSoftDeletedAsync(Expression<Func<Brand, bool>> predicate);
        Task<List<Brand>> GetAllIncludeSoftDeletedAsync(Expression<Func<Brand, bool>> predicate);
    }
}
