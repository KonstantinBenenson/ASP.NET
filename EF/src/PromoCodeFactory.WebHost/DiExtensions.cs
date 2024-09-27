using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Enums;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Configurations;
using System;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost
{
    public static class DiExtensions
    {
        public static async Task SeedDataAsync(this IHost host)
        {
            await using var services = host.Services.CreateAsyncScope();
            var serviceProvider = services.ServiceProvider;
            var db = serviceProvider.GetRequiredService<DatabaseContext>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Data Seed");

            try
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"An Error occured while recreating and seeding database: {ex.Message}");
            }
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IExtendedRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IExtendedRepository<Customer>, CustomersRepository>();
            services.AddScoped<IRepository<Role>, EfCoreRepository<Role>>();
            services.AddScoped<IExtendedRepository<Preference>, PreferencesRepository>();
            services.AddScoped<IRepository<PromoCode>, PromoCodesRepository>();
        }

        public static void AddDbContextConfigured<T>(this IServiceCollection services, DatabaseProviders provider) where T : DbContext
        {
            using var serviceProvider = services.BuildServiceProvider();
            var appSettings = serviceProvider.GetService<IOptions<AppSettings>>()!.Value;

            services.AddDbContext<T>(options =>
            {
                options.UseSqlite(appSettings.ConnectionStrings[provider.ToString()]);
            });
        }
    }
}
