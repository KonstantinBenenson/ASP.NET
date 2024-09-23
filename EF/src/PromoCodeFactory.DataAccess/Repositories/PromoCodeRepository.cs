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
    public class PromoCodeRepository : EfCoreRepository<PromoCode>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<PromoCodeRepository> _logger;

        public PromoCodeRepository(DatabaseContext dbContext, ILogger<PromoCodeRepository> logger) : base(dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task CreateAsync(PromoCode promoCode, CancellationToken token)
        {
            using (var scope = await _dbContext.Database.BeginTransactionAsync(token))
            {
                var customersToAddPromoCodes = _dbContext.Customers
                    .Where(c => c.CustomersPreferences.Any(cp => cp.Preference.Name == promoCode.Preference.Name))
                    .Include(c => c.CustomersPreferences)
                    .ThenInclude(cp => cp.Preference);

                foreach (var customer in customersToAddPromoCodes)
                {
                    if (customer.PromoCodes is null)
                        customer.PromoCodes = new List<PromoCode>();
                    promoCode.CustomerId = customer.Id;
                    
                    if (promoCode.PreferenceId == Guid.Empty)
                        promoCode.PreferenceId = customer.CustomersPreferences
                            .Where(cp => cp.Preference.Name == promoCode.Preference.Name).FirstOrDefault().PreferenceId;

                    customer.PromoCodes.Add(promoCode);
                }
                try
                {
                    _dbContext.Customers.UpdateRange(customersToAddPromoCodes);
                    //await _dbContext.PromoCodes.AddAsync(promoCode, token);
                    await _dbContext.SaveChangesAsync(token);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error occured while updating customer with a new PromoCode.");
                }
                catch (Exception ex) 
                {
                    
                }

                await scope.CommitAsync(token);
            }
        }
    }
}
