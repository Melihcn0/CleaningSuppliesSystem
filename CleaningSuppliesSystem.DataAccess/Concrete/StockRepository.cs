using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;

public class StockRepository : GenericRepository<Product>, IStockRepository
{
    private readonly CleaningSuppliesSystemContext _context;

    public StockRepository(CleaningSuppliesSystemContext context) : base(context)
    {
        _context = context;
    }


    public async Task<bool> AssignStockAsync(int productId, int quantity, bool isStockIn)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        if (isStockIn)
        {
            product.StockQuantity = (product.StockQuantity ?? 0) + quantity;
        }
        else
        {
            if ((product.StockQuantity ?? 0) < quantity)
                return false;

            product.StockQuantity -= quantity;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Brand)
                .ThenInclude(b => b.Category)
                    .ThenInclude(c => c.SubCategory)
                        .ThenInclude(sc => sc.TopCategory)
            .Where(p => !p.IsDeleted && p.IsShown)
            .ToListAsync();
    }
}
