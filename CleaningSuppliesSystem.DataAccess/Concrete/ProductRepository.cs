using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly CleaningSuppliesSystemContext _cleaningSuppliesContext;
        public ProductRepository(CleaningSuppliesSystemContext _context) : base(_context)
        {
            _cleaningSuppliesContext = _context;
        }
        public async Task<List<Product>> GetProductsWithCategoriesAsync()
        {
            return await _cleaningSuppliesContext.Products.Include(p => p.Category).ToListAsync();
        }
        public async Task<Product> GetByIdAsyncWithCategory(int id)
        {
            return await _cleaningSuppliesContext.Products.Include(p => p.Category) .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}