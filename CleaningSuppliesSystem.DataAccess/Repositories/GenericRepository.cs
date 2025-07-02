using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Repositories
{
    public class GenericRepository<T>(CleaningSuppliesSystemContext _context) : IRepository<T> where T : class
    {
        private DbSet<T> Table { get => _context.Set<T>(); }

        public async Task<List<T>> GetListAsync()
        {
            return await Table.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Table.FindAsync(id);
        }

        public async Task<T> GetByFilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync();
        }

        public async Task CreateAsync(T entity)
        {
            await Table.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            Table.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await Table.FindAsync(id);
            Table.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await Table.CountAsync();
        }

        public async Task<int> FilteredCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.AnyAsync(predicate);
        }
    }
}
