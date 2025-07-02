using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetListAsync();  // Tüm verileri getirir
        Task<T> GetByIdAsync(int id);  // Id ile veri getirir
        Task<T> GetByFilterAsync(Expression<Func<T, bool>> predicate);  // Şarta göre tek veri
        Task<List<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate); // Şarta göre çoklu veri
        Task CreateAsync(T entity);  // Ekleme işlemi
        Task UpdateAsync(T entity);  // Güncelleme
        Task DeleteAsync(int id);  // Silme
        Task<int> CountAsync();  // Tüm veri sayısı
        Task<int> FilteredCountAsync(Expression<Func<T, bool>> predicate); // Şartlı sayım
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate); // Veri var mı kontrolü
    }
}
