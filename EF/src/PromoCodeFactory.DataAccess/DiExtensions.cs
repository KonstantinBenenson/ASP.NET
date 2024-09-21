using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess
{
    public static class DiExtensions
    {
        public static async Task EnsureDeletedAndSeeded(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
            await DatabaseContext.Seed(services);
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Employee>, EfCoreRepository<Employee>>();
            services.AddScoped<IRepository<Customer>, EfCoreRepository<Customer>>();
            services.AddScoped<IRepository<Role>, EfCoreRepository<Role>>();
            services.AddScoped<IRepository<Preference>, EfCoreRepository<Preference>>();
            services.AddScoped<IRepository<PromoCode>, EfCoreRepository<PromoCode>>();
        }
    }
}
