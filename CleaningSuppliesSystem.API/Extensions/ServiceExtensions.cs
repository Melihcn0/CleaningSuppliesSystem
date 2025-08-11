using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Concrete;
using CleaningSuppliesSystem.Business.Configurations;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace CleaningSuppliesSystem.API.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            // DbContext ekle
            services.AddDbContext<CleaningSuppliesSystemContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
            });

            // Identity servisini ekle (UserManager ve RoleManager otomatik eklenir)
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<CleaningSuppliesSystemContext>()
                .AddDefaultTokenProviders();

            // Repository ve servis kayıtları
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderItemService, OrderItemManager>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderService, OrderManager>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IInvoiceService, InvoiceManager>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            services.Configure<JwtTokenOptions>(configuration.GetSection("TokenOptions"));

            services.AddScoped<IJwtService, JwtManager>();
            services.AddScoped<IEmailService, EmailManager>();

            services.AddScoped<IUserService, UserManager>();

            services.AddScoped<IRoleService, RoleManager>();

            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IBrandService, BrandManager>();
            services.AddScoped<IBrandRepository, BrandRepository>();

            services.AddScoped<ISubCategoryService, SubCategoryManager>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

            services.AddScoped<ITopCategoryService, TopCategoryManager>();
            services.AddScoped<ITopCategoryRepository, TopCategoryRepository>();

            services.AddScoped<IStockService, StockManager>();
            services.AddScoped<IStockRepository, StockRepository>();

            services.AddScoped<IFinanceService, FinanceManager>();
            services.AddScoped<IFinanceRepository, FinanceRepository>();

            services.AddScoped<IBannerService, BannerManager>();
            services.AddScoped<IBannerRepository, BannerRepository>();

            services.AddScoped<ISecondaryBannerService, SecondaryBannerManager>();
            services.AddScoped<ISecondaryBannerRepository, SecondaryBannerRepository>();

            services.AddScoped<IServiceIconService, ServiceIconManager>();
            services.AddScoped<IServiceIconRepository, ServiceIconRepository>();

            services.AddScoped<IServiceService, ServiceManager>();
            services.AddScoped<IServiceRepository, ServiceRepository>();

            services.AddScoped<IPdfService, PdfManager>();

            services.AddHttpClient();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAuthorization();

            return services;
        }
    }
}
