using System.Threading.Tasks;
using System.Threading;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IExtendedRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<T> GetByNameAsync(string name, CancellationToken token);
    }
}
