using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(builder =>
            {
                builder.Property(x => x.FirstName).HasMaxLength(25).IsRequired();
                builder.Property(x => x.LastName).HasMaxLength(25).IsRequired();
                builder.Property(x => x.Email).HasMaxLength(30).IsRequired();
                builder.HasIndex(x => x.Email).IsUnique();

                //builder.HasData(FakeDataFactory.Customers);
                //j => j.HasKey(cp => new { cp.PreferenceId, cp.CustomerId });
                //j.HasData(FakeDataFactory.CustomersPreferences);

                builder.HasMany(x => x.Preferences).WithMany(x => x.Customers)
                .UsingEntity<CustomerPreference>(builder => {
                    builder.HasOne(c => c.Customer).WithMany().HasForeignKey(x => x.CustomerId);
                    builder.HasOne(c => c.Preference).WithMany().HasForeignKey(x => x.PreferenceId);
                    //builder.HasKey(cp => new { cp.PreferenceId, cp.CustomerId });
                    builder.HasData(FakeDataFactory.CustomersPreferences);
                });
                //UsingEntity<CustomerPreference>(
                //    builder =>
                //    {
                //        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
                //        builder.HasOne(x => x.Preference).WithMany().HasForeignKey(x => x.PreferenceId);
                //        builder.HasData(FakeDataFactory.CustomersPreferences);
                //    })
            });

            modelBuilder.Entity<Employee>().HasData(FakeDataFactory.Employees);
            modelBuilder.Entity<Customer>().HasData(FakeDataFactory.Customers);
            modelBuilder.Entity<Preference>().HasData(FakeDataFactory.Preferences);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=PromoCodeFactory");
        }

        public static async Task Seed(IServiceCollection serviceCollection)
        {
            using var scope = serviceCollection.BuildServiceProvider().CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            if (!await db.Customers.AnyAsync())
            {
                await db.Customers.AddRangeAsync(FakeDataFactory.Customers);
                await db.SaveChangesAsync();
            }

        }
    }
}
