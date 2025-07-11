using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Concrete;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.WebUI.Services.UserServices;
using CleaningSuppliesSystem.WebUI.Services.RoleServices;
using Microsoft.AspNetCore.Builder;
using CleaningSuppliesSystem.WebUI.Services.EmailServices;
using FluentValidation;
using static CleaningSuppliesSystem.Business.Validators.Validators;

var builder = WebApplication.CreateBuilder(args);

// DbContext'i Identity'den önce ekle
builder.Services.AddDbContext<CleaningSuppliesSystemContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});


builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<CleaningSuppliesSystemContext>().AddDefaultTokenProviders();
builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFinanceService, FinanceManager>();
builder.Services.AddScoped<IFinanceRepository, FinanceRepository>();
builder.Services.AddScoped<IStockEntryService, StockEntryManager>();
builder.Services.AddScoped<IStockEntryRepository, StockEntryRepository>();
builder.Services.AddScoped<IMailService, MailManager>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddValidatorsFromAssemblyContaining<ForgotPasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordValidator>();


builder.Services.ConfigureApplicationCookie(cfg =>
{
    cfg.LoginPath = "/Login/SignIn";
    cfg.LogoutPath = "/Login/Logout";
    cfg.AccessDeniedPath = "/ErrorPage/AccessDenied";
    //cfg.Cookie.Name = "CleaningSuppliesSystemCookie";
    //cfg.ExpireTimeSpan = TimeSpan.FromDays(30);
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); // kilit süresi
    options.Lockout.MaxFailedAccessAttempts = 10; // maksimum deneme
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(15); // 15 dakikalýk geçerlilik
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseStatusCodePagesWithReExecute("/ErrorPage/NotFound404/");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
