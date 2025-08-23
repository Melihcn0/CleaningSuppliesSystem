using CleaningSuppliesSystem.API.Extensions;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Concrete;
using CleaningSuppliesSystem.Business.Configurations;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using static CleaningSuppliesSystem.Business.Validators.Validators;
using QuestPDF;
using System.ComponentModel;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddProjectDependencies(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ForgotPasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateFinanceValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateFinanceValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateDiscountValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBrandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBrandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSubCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSubCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTopCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTopCategoryValidator>();


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// ... diðer servis ekleme kodlarýnýz

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7020")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
// ...

builder.Services.AddHttpContextAccessor();



var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<JwtTokenOptions>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Key)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name
    };
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(5);
});


builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT ile yetkilendirme. Format: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.SwaggerDoc("TopCategories", new OpenApiInfo { Title = "TopCategory Management", Version = "v1" });
    c.SwaggerDoc("SubCategories", new OpenApiInfo { Title = "SubCategory Management", Version = "v1" });
    c.SwaggerDoc("Categories", new OpenApiInfo { Title = "Category Management", Version = "v1" });
    c.SwaggerDoc("Brands", new OpenApiInfo { Title = "Brand Management", Version = "v1" });
    c.SwaggerDoc("Products", new OpenApiInfo { Title = "Product Management", Version = "v1" });
    c.SwaggerDoc("Finances", new OpenApiInfo { Title = "Finance Management", Version = "v1" });
    c.SwaggerDoc("Invoices", new OpenApiInfo { Title = "Invoice Management", Version = "v1" });
    c.SwaggerDoc("Stock", new OpenApiInfo { Title = "Stock Management", Version = "v1" });
    c.SwaggerDoc("User", new OpenApiInfo { Title = "User Management", Version = "v1" });
    c.SwaggerDoc("Order", new OpenApiInfo { Title = "Order Management", Version = "v1" });
    c.SwaggerDoc("Roles", new OpenApiInfo { Title = "Role Management", Version = "v1" });
    c.SwaggerDoc("Home", new OpenApiInfo { Title = "Home Management", Version = "v1" });
    c.SwaggerDoc("Customer", new OpenApiInfo { Title = "Customer Management", Version = "v1" });
    c.SwaggerDoc("Location", new OpenApiInfo { Title = "Location Management", Version = "v1" });

    // Sadece eþleþen GroupName'leri dahil et
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        var groupName = apiDesc.GroupName;
        return groupName == docName;
    });

    c.TagActionsBy(api => new[] { api.GroupName });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/TopCategories/swagger.json", "TopCategory Management");
        x.SwaggerEndpoint("/swagger/SubCategories/swagger.json", "SubCategory Management");
        x.SwaggerEndpoint("/swagger/Categories/swagger.json", "Category Management");
        x.SwaggerEndpoint("/swagger/Brands/swagger.json", "Brand Management");
        x.SwaggerEndpoint("/swagger/Products/swagger.json", "Product Management");
        x.SwaggerEndpoint("/swagger/Finances/swagger.json", "Finance Management");
        x.SwaggerEndpoint("/swagger/Invoices/swagger.json", "Invoice Management");
        x.SwaggerEndpoint("/swagger/Stock/swagger.json", "Stock Management");
        x.SwaggerEndpoint("/swagger/User/swagger.json", "User Management");
        x.SwaggerEndpoint("/swagger/Order/swagger.json", "Order Management");
        x.SwaggerEndpoint("/swagger/Roles/swagger.json", "Role Management");
        x.SwaggerEndpoint("/swagger/Home/swagger.json", "Home Management");
        x.SwaggerEndpoint("/swagger/Customer/swagger.json", "Customer Management");
        x.SwaggerEndpoint("/swagger/Location/swagger.json", "Location Management");

        x.RoutePrefix = "docs";
    });

}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
