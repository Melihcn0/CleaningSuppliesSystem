using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.DataAccess.Context
{
    public class CleaningSuppliesSystemContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public CleaningSuppliesSystemContext(DbContextOptions options) : base(options)
        {
        }

        // 👉 DbSet tanımları
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<StockEntry> StockEntries { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}