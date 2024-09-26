using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    /// <summary>
    /// This implementation was created in order to add the Include method to the queries.
    /// </summary>
    public class EmployeeRepository : EfCoreRepository<Employee>
    {
        private readonly DatabaseContext _dbContext;

        public EmployeeRepository(DatabaseContext dbContext, ILogger<Employee> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
        }

        public override async Task<Employee> GetByNameAsync(string name, CancellationToken token)
        {
            return await _dbContext.Employees.FirstOrDefaultAsync(e => e.FirstName == name, token);
        }

    }
}
