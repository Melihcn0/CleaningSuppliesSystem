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

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<TopCategory> TopCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<SecondaryBanner> SecondaryBanners { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceIcon> ServiceIcons { get; set; }
        public DbSet<CustomerIndividualAddress> CustomerIndividualAddresses { get; set; }
        public DbSet<CustomerCorporateAddress> CustomerCorporateAddresses { get; set; }
        public DbSet<LocationCity> LocationCitys { get; set; }
        public DbSet<LocationDistrict> LocationDistricts { get; set; }
        public DbSet<CompanyAddress> CompanyAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Brand>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Brands)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .ToTable(tb => tb.HasTrigger("TR_DeleteOrder_WhenNoOrderItems"));

        }
    }
}