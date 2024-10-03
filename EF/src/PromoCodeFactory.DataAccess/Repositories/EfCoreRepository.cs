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
            return await _dbSet.ToListAsync(token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, token);
        }
        public async Task<List<T>> GetByFilterAsync(Expression<Func<T, bool>> expression, CancellationToken token)
        {
            return await _dbSet.Where(expression).ToListAsync(token);
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
            var (result, storedEntity) = await TryFindEntity(id, token);
            if (!result) return;

            _dbSet.Remove(storedEntity!);
            await _dbContext.SaveChangesAsync(token);
        }

        #region Utils

        /// <summary>
        /// Tries to find an Entity with the provided Key. Otherwise throws the ArgumentException.
        /// </summary>
        private async Task<(bool Result, T? entity)> TryFindEntity(Guid id, CancellationToken token)
        {
            var entity = await _dbSet.Where(e => e.Id == id).FirstOrDefaultAsync(token);
            var result = entity is not null;
            if (!result)
            {
                _logger.LogWarning($"Entity {typeof(T)} not found", nameof(id));
            }
            return (result, entity);
        }

        #endregion
    }
}
