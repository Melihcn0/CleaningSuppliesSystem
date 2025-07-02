using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IGenericService<T> where T : class
    {
        Task<List<T>> TGetListAsync();
        Task<T> TGetByIdAsync(int id);
        Task<T> TGetByFilterAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> TGetFilteredListAsync(Expression<Func<T, bool>> predicate);

        Task TCreateAsync(T entity);
        Task TUpdateAsync(T entity);
        Task TDeleteAsync(int id);

        Task<int> TCountAsync();
        Task<int> TFilteredCountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> TAnyAsync(Expression<Func<T, bool>> predicate);
    }
}
