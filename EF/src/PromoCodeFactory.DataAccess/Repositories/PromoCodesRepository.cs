﻿using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class PromoCodesRepository : EfCoreRepository<PromoCode>
    {
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<PromoCodesRepository> _logger;

        public PromoCodesRepository(DatabaseContext dbContext, ILogger<PromoCodesRepository> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task CreateAsync(PromoCode promoCode, CancellationToken token)
        {
            await _dbContext.PromoCodes.AddAsync(promoCode, token);
            await _dbContext.SaveChangesAsync(token);
        }
    }
}
