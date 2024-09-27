using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfCoreRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DatabaseContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private ILogger<EfCoreRepository<T>> _logger;

        public EfCoreRepository(DatabaseContext dbContext, ILogger<EfCoreRepository<T>> logger)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _logger = logger;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token)
        {
            // we use AsNoTrackingWithIdentityResolution because potentially we might return references to different instances of the same Entity
            // (f.e. a Customer can be referenced inside of multiple Promocodes)
            return await _dbSet.AsNoTrackingWithIdentityResolution().ToListAsync(token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public virtual async Task CreateAsync(T entity, CancellationToken token)
        {
            await _dbSet.AddAsync(entity, token);
            await _dbContext.SaveChangesAsync(token);
        }

        public virtual async Task UpdateAsync(Guid id, T entity, CancellationToken token)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(token);
        }

        public virtual async Task DeleteByIdAsync(Guid id, CancellationToken token)
        {
            var storedEntity = await TryFindEntity(id, token);
            if (storedEntity is null) return;

            _dbSet.Entry(storedEntity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync(token);
        }

        #region Utils

        /// <summary>
        /// Tries to find an Entity with the provided Key. Otherwise throws the ArgumentException.
        /// </summary>
        private async Task<T> TryFindEntity(Guid id, CancellationToken token)
        {
            if (await _dbSet.Where(e => e.Id == id).FirstOrDefaultAsync(token) is not T storedEntity)
            {
                _logger.LogWarning($"Entity {typeof(T)} not found", nameof(id));
                return null;
            }
            return storedEntity;
        }

        public async Task<List<T>> GetByFilterAsync(Expression<Func<T, bool>> expression, CancellationToken token)
        {
            return await _dbSet.Where(expression).ToListAsync(token);
        }

        #endregion
    }
}
