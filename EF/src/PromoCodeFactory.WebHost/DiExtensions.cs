using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
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
            services.AddScoped<IRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IRepository<Customer>, CustomersRepository>();
            services.AddScoped<IRepository<Role>, EfCoreRepository<Role>>();
            services.AddScoped<IRepository<Preference>, PreferencesRepository>();
            services.AddScoped<IRepository<PromoCode>, PromoCodesRepository>();
        }
    }
}
