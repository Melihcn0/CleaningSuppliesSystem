using Microsoft.AspNetCore.Identity;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using CleaningSuppliesSystem.WebUI.Services.TokenServices;
using CleaningSuppliesSystem.WebUI.Handlers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// HttpContext
builder.Services.AddHttpContextAccessor();

// TokenService ve DelegatingHandler
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddTransient<TokenHandler>();

// HttpClient + TokenHandler (JWT ile API'ye gidecek client)
builder.Services.AddHttpClient("CleaningSuppliesSystemClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7200/api/");
}).AddHttpMessageHandler<TokenHandler>();

// Cookie Authentication (Web arayüzü için)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
    {
        opt.LoginPath = "/Login/SignIn";
        opt.LogoutPath = "/Login/Logout";
        opt.AccessDeniedPath = "/ErrorPage/AccessDenied";
        opt.Cookie.SameSite = SameSiteMode.Strict;
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opt.Cookie.Name = "CleaningSuppliesSystemCookie";
    });

// Opsiyonel: Identity ayarları
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 15;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(15);
});

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // 20 dk işlem yapılmazsa session sona erer
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // GDPR için gerekli
});

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Session middleware aktif edildi

// Session timeout kontrol middleware'i
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userName = context.Session.GetString("Identifier");
        var userRole = context.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userRole))
        {
            // Session boşalmışsa, kullanıcıyı oturumdan at
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Önemli: Oturumdan atıldıktan sonra kullanıcıyı hemen giriş sayfasına yönlendir.
            // Bu, menüdeki yanlış linkin görülmesini engeller.
            context.Response.Redirect("/Login/SignIn");
            return;
        }
    }
    await next();
});


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
