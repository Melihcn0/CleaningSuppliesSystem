using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Concrete;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext ekle
builder.Services.AddDbContext<CleaningSuppliesSystemContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

// Identity servisini ekle (UserManager ve RoleManager otomatik eklenir)
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
   
})
.AddEntityFrameworkStores<CleaningSuppliesSystemContext>()
.AddDefaultTokenProviders();

// Repository ve servis kayýtlarý
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IStockEntryService, StockEntryManager>();
builder.Services.AddScoped<IStockEntryRepository, StockEntryRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemManager>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderService, OrderManager>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceManager>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IMailService, MailManager>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
