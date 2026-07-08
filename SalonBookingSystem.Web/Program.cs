using SalonBookingSystem.Web.Configuration;
using SalonBookingSystem.Web.Services;

namespace SalonBookingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Required for ApiService to read the session cookie and forward it
            builder.Services.AddHttpContextAccessor();

            // Session — stores AuthCookie, CustomerId, UserEmail after login
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // HttpClient for ApiService.
            // Handler settings:
            //   - UseCookies = false  → we manage the cookie header manually (session-stored)
            //   - AllowAutoRedirect = false → we handle redirects explicitly
            //   - DangerousAcceptAnyServerCertificateValidator → trusts localhost dev cert
            builder.Services.AddHttpClient<IApiService, ApiService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseCookies = false,
                    AllowAutoRedirect = false,
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });

            builder.Services.Configure<ApiSettings>(
                builder.Configuration.GetSection(ApiSettings.SectionName));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
