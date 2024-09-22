using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// This implementation was created in order to add the Include method to the queries.
    /// </summary>
    public class CustomersRepository : EfCoreRepository<Customer>
    {
        private readonly DatabaseContext _dbContext;

        public CustomersRepository(DatabaseContext dbContext) : base(dbContext)
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

        public override async Task DeleteByIdAsync(Guid id)
        {
            if (_dbContext.PromoCodes.Where(p => p.CustomerId == id) is IQueryable<PromoCode> promoCodesToDelete)
            {
                _dbContext.PromoCodes.RemoveRange(promoCodesToDelete);
            }

            // TODO: here we actually make double check if the provided Id actually belongs to a Customer... 
            await base.DeleteByIdAsync(id);
        }
    }
}
