// Program.cs
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using CleaningSuppliesSystem.WebUI.Services.TokenServices;
using CleaningSuppliesSystem.WebUI.Handlers;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CleaningSuppliesSystem.DTO.DTOs.LoginDtos;
using System.Text;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using CleaningSuppliesSystem.WebUI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// HttpContext
builder.Services.AddHttpContextAccessor();

// TokenService ve DelegatingHandler
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddTransient<TokenHandler>();
builder.Services.AddScoped<PaginationHelper>();

// TempData için Session desteği ekleniyor
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

        opt.Cookie.Name = "CleaningSuppliesSystemCookie";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opt.Cookie.SameSite = SameSiteMode.Strict;

        // Cookie süresi hep 20 dk, sliding expiration kapalı
        opt.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        opt.SlidingExpiration = false; // artık süre yenilenmeyecek
        var maxSession = TimeSpan.FromHours(1); // maksimum oturum süresi kontrolü hâlâ geçerli

        opt.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var handler = new JwtSecurityTokenHandler();

                async Task LogoutAndUpdateUser(string message)
                {
                    var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                    {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var user = await userManager.FindByIdAsync(userIdClaim);
                        if (user != null)
                        {
                            user.LastLogoutAt = DateTime.UtcNow;
                            await userManager.UpdateAsync(user);
                        }
                    }

                    var tempDataFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                    var tempData = tempDataFactory.GetTempData(context.HttpContext);
                    tempData["SessionExpiredWarning"] = message;

                    // Principal'ı reddet - redirect işlemini LoginController'a bırak
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.HttpContext.Response.Redirect("/Login/SignIn");

                }

                if (string.IsNullOrEmpty(token))
                {
                    await LogoutAndUpdateUser("Oturumunuzun süresi dolmuştur. Lütfen tekrar giriş yapınız.");
                    return;
                }

                try
                {
                    var jwt = handler.ReadJwtToken(token);

                    if (jwt.ValidTo < DateTime.UtcNow)
                    {
                        await LogoutAndUpdateUser("Oturumunuzun süresi dolmuştur. Lütfen tekrar giriş yapınız.");
                        return;
                    }

                    var issuedClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value;
                    if (issuedClaim != null && long.TryParse(issuedClaim, out long iatUnix))
                    {
                        var issuedTime = DateTimeOffset.FromUnixTimeSeconds(iatUnix).UtcDateTime;
                        if (DateTime.UtcNow - issuedTime > TimeSpan.FromHours(1))
                        {
                            await LogoutAndUpdateUser("Maksimum oturum süreniz dolmuştur. Lütfen tekrar giriş yapınız.");
                            return;
                        }
                    }
                }
                catch
                {
                    await LogoutAndUpdateUser("Oturumunuz geçersiz hale gelmiştir. Lütfen tekrar giriş yapınız.");
                }
            }
        };

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

// Session middleware'i authentication'dan önce eklenmeli
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/ErrorPage/NotFound404/");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();