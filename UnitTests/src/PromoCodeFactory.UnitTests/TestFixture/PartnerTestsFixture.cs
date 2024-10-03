using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.TestFixture
{
    public class PartnerTestsFixture : IDisposable
    {
        public PartnersController PartnersController { get; }
        public IRepository<Partner> PartnersRepository { get; private set; }
        public IServiceProvider ServiceProvider { get; }
        public DataContext DataContext { get; }
        public PartnerTestsFixture()
        {
            var services = new ServiceCollection();

            services.AddScoped<PartnersController>();
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddDbContext<DataContext>(x =>
            {
                x.UseSqlite("DataSource=:memory:;cache=shared");
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            ServiceProvider = services.BuildServiceProvider();
            DataContext = ServiceProvider.GetRequiredService<DataContext>();
            // Open Connection and Ensure that all tables are created in DB
            DataContext.Database.OpenConnection();
            DataContext.Database.EnsureCreated();

            Seed().GetAwaiter().GetResult();

            PartnersController = ServiceProvider.GetRequiredService<PartnersController>();
        }

        private async Task Seed()
        {
            PartnersRepository = ServiceProvider.GetRequiredService<IRepository<Partner>>();
            await DataContext.Partners.AddRangeAsync(FakeDataFactory.Partners);
            await DataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            DataContext.ChangeTracker.Clear();
            DataContext.Database.CloseConnection();
            DataContext.Dispose();
        }
    }
}
