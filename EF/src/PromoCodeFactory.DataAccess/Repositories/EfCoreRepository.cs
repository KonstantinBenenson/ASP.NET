using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Gets all the entries of the provided Entity.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Gets an entry, filtered with provided Id. Returns null if nothing is found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
