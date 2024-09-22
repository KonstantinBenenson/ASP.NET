using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfCoreRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DatabaseContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public EfCoreRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(Guid id, T entity)
        {
            var storedEntity = TryFindEntityOrThrow(id);
            _dbSet.Entry(storedEntity).CurrentValues.SetValues(entity);
            _dbSet.Entry(storedEntity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteByIdAsync(Guid id)
        {
            var storedEntity = TryFindEntityOrThrow(id);
            _dbSet.Entry(storedEntity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        #region Utils

        /// <summary>
        /// Tries to find an Entity with the provided Key. Otherwise throws the ArgumentException.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        private T TryFindEntityOrThrow(Guid id)
        {
            if (_dbSet.Where(e => e.Id == id) is not T storedEntity)
            {
                throw new ArgumentException($"Entity {typeof(T)} not found", nameof(id));
            }
            return storedEntity;
        }

        #endregion
    }
}
