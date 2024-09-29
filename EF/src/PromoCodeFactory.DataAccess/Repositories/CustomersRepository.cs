using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// This implementation was created in order to add the Include method to the queries.
    /// </summary>
    public class CustomersRepository : EfCoreRepository<Customer>, IExtendedRepository<Customer>
    {
        private readonly ILogger<CustomersRepository> _logger;
        private readonly DatabaseContext _dbContext;

        public CustomersRepository(DatabaseContext dbContext, ILogger<CustomersRepository> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken token)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomersPreferences)
                .ThenInclude(cp => cp.Preference)
                .Include(c => c.PromoCodes)
                .ToListAsync(token);
        }

        public override async Task<Customer> GetByIdAsync(Guid id, CancellationToken token)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomersPreferences)
                .ThenInclude(cp => cp.Preference)
                .Include(c => c.PromoCodes)
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }

        public async Task<Customer> GetByNameAsync(string name, CancellationToken token)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(e => e.FirstName == name, token);
        }

        public override async Task CreateAsync(Customer entity, CancellationToken token)
        {
            await _dbContext.Customers.AddAsync(entity, token);
            await _dbContext.SaveChangesAsync(token);
        }

        public override async Task UpdateAsync(Guid id, Customer entity, CancellationToken token)
        {
            var customer = await GetByIdAsync(id, token);
            if (customer is not null)
            {
                customer.CustomersPreferences = new List<CustomerPreference>();
                (customer.CustomersPreferences as List<CustomerPreference>)!.AddRange(entity.CustomersPreferences);   
                _dbContext.Entry(customer).CurrentValues.SetValues(entity);
                _dbContext.Entry(customer).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync(token);
            }
        }
    }
}
