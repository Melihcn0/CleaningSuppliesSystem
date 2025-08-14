using Microsoft.AspNetCore.Identity;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using CleaningSuppliesSystem.WebUI.Services.TokenServices;
using CleaningSuppliesSystem.WebUI.Handlers;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["AccessToken"];
    var handler = new JwtSecurityTokenHandler();

    if (!string.IsNullOrEmpty(token))
    {
        var jwt = handler.ReadJwtToken(token);

        if (jwt.ValidTo < DateTime.UtcNow ||
            context.User.Identity?.IsAuthenticated == true &&
            context.User.FindFirst("Identifier") == null)
        {
            context.Response.Cookies.Delete("AccessToken");
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.User = new ClaimsPrincipal(); // Authenticated false gibi davranır
        }
    }

    await next();

    // Eğer 404 ve 401 ise ve hâlâ authenticated görünüyorsa → login'e yönlendir
    if ((context.Response.StatusCode == 404 || context.Response.StatusCode == 401) &&
        context.User.Identity?.IsAuthenticated == true &&
        !context.Response.HasStarted)
    {
        context.Response.Redirect("~/Login/SignIn");
    }
});


app.UseStatusCodePagesWithReExecute("/ErrorPage/NotFound404/");


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
