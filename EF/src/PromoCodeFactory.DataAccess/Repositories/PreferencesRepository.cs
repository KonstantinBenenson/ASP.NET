using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// This implementation was created in order to add the Include method to the queries.
    /// </summary>
    public class PreferencesRepository : EfCoreRepository<Preference>, IExtendedRepository<Preference>
    {
        private readonly DatabaseContext _dbContext;

        public PreferencesRepository(DatabaseContext dbContext, ILogger<PreferencesRepository> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<Preference> GetByNameAsync(string name, CancellationToken token)
        {
            return await _dbContext.Preferences.FirstOrDefaultAsync(e => e.Name == name, token);
        }
    }
}
