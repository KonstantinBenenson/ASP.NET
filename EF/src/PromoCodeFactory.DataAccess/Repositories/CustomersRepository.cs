using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// This implementation was created in order to add the Include method to the queries.
    /// </summary>
    public class CustomersRepository : EfCoreRepository<Customer>
    {
        private readonly DatabaseContext _dbContext;

        public CustomersRepository(DatabaseContext dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers
                .Include(c => c.CustomersPreferences)
                .ThenInclude(cp => cp.Preference)
                .ToListAsync();
        }

        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomersPreferences)
                .ThenInclude(cp => cp.Preference)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
