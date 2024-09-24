using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        //public override async Task CreateAsync(PromoCode promoCode, CancellationToken token)
        //{
        //    using (var scope = await _dbContext.Database.BeginTransactionAsync(token))
        //    {
        //        var customersToAddPromoCodes = _dbContext.Customers
        //            .Where(c => c.CustomersPreferences.Any(cp => cp.Preference.Name == promoCode.Preference.Name))
        //            .Include(c => c.CustomersPreferences)
        //            .ThenInclude(cp => cp.Preference);

        //        var promoCodeBatch = new List<PromoCode>();

        //        foreach (var customer in customersToAddPromoCodes)
        //        {
        //            //if (customer.PromoCodes is null)
        //            //    customer.PromoCodes = new List<PromoCode>();
        //            promoCode.CustomerId = customer.Id;

        //            if (promoCode.PreferenceId == Guid.Empty)
        //                promoCode.PreferenceId = customer.CustomersPreferences
        //                    .Where(cp => cp.Preference.Name == promoCode.Preference.Name).FirstOrDefault().PreferenceId;
        //            if (promoCode.PartnerManagerId == Guid.Empty)
        //                promoCode.PartnerManagerId = await _dbContext.Employees.Select(e => e.Id).FirstOrDefaultAsync(token);

        //            // Add to batch
        //            promoCodeBatch.Add(promoCode);

        //            customer.PromoCodes = new List<PromoCode>() { promoCode };
        //        }
        //        try
        //        {
        //            await _dbContext.PromoCodes.AddRangeAsync(promoCodeBatch, token);
        //            _dbContext.Customers.UpdateRange(customersToAddPromoCodes);
        //            await _dbContext.SaveChangesAsync(token);

        //            var result = await customersToAddPromoCodes.Include(c => c.PromoCodes).FirstOrDefaultAsync(token);
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            _logger.LogError(ex, "Error occured while updating customer with a new PromoCode.");
        //        }
        //        catch (Exception ex) 
        //        {

        //        }

        //        await scope.CommitAsync(token);
        //    }
        //}
        public override async Task CreateAsync(PromoCode promoCode, CancellationToken token)
        {
            var preferenceId = await _dbContext.Preferences.Where(p => p.Name == promoCode.Preference.Name).Select(p => p.Id).SingleOrDefaultAsync(token);
            var employeeId = await _dbContext.Employees.Where(e => e.FirstName == promoCode.PartnerName).Select(p => p.Id).SingleOrDefaultAsync(token);
            promoCode.PreferenceId = preferenceId;
            promoCode.PartnerManagerId = employeeId;
            await _dbContext.PromoCodes.AddAsync(promoCode, token);
            await _dbContext.SaveChangesAsync(token);
        }
    }
}
