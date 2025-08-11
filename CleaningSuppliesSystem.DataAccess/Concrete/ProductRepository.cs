using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly CleaningSuppliesSystemContext _context;
    public ProductRepository(CleaningSuppliesSystemContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Product product)
    {
        product.CreatedDate = DateTime.UtcNow;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdatedDate = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Product product)
    {
        product.IsDeleted = true;
        product.DeletedDate = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task UndoSoftDeleteAsync(Product product)
    {
        product.IsDeleted = false;
        product.DeletedDate = null;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
    }
    public async Task<List<Product>> GetDeletedProductAsync()
    {
        return await _context.Products
            .Include(p => p.Brand)
                .ThenInclude(b => b.Category)
                    .ThenInclude(c => c.SubCategory)
                        .ThenInclude(sc => sc.TopCategory)
            .Where(p => p.IsDeleted)
            .ToListAsync();
    }


    public async Task<List<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Brand)
                .ThenInclude(b => b.Category)
                    .ThenInclude(c => c.SubCategory)
                        .ThenInclude(sc => sc.TopCategory)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }
    public async Task<List<Product>> GetActiveByBrandsIdAsync(int brandId)
    {
        return await _context.Products
            .Where(sc => sc.BrandId == brandId && !sc.IsDeleted)
            .ToListAsync();
    }
    public async Task SoftDeleteRangeAsync(List<int> ids)
    {
        var products = await _context.Products
            .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
            .ToListAsync();

        foreach (var product in products)
        {
            product.IsDeleted = true;
        }

        _context.Products.UpdateRange(products);
        await _context.SaveChangesAsync();
    }

    public async Task UndoSoftDeleteRangeAsync(List<int> ids)
    {
        var products = await _context.Products
            .Where(c => ids.Contains(c.Id) && c.IsDeleted)
            .ToListAsync();

        foreach (var product in products)
        {
            product.IsDeleted = false;
        }

        _context.Products.UpdateRange(products);
        await _context.SaveChangesAsync();
    }

    public async Task PermanentDeleteRangeAsync(List<int> ids)
    {
        var products = await _context.Products
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();

        _context.Products.RemoveRange(products);
        await _context.SaveChangesAsync();
    }


}
