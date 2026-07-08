using SalonBookingSystem.API.Converters;
using SalonBookingSystem.API.Middleware;
using SalonBookingSystem.Application.DependencyInjection;
using SalonBookingSystem.Infrastructure.DependencyInjection;
using SalonBookingSystem.Persistence.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;
using ApplicationUser = SalonBookingSystem.Persistence.Context.ApplicationUser;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
});

// Add CORS - allow all origins for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Identity cookie options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.LoginPath = "/api/v1/auth/login";
    options.AccessDeniedPath = "/api/v1/auth/access-denied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
