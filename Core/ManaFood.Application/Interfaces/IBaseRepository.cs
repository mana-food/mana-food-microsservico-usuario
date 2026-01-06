using System.Linq.Expressions;
using ManaFood.Application.Shared;
using ManaFood.Domain.Entities;

namespace ManaFood.Application.Interfaces;
public interface IBaseRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAll(CancellationToken cancellationToken);
    Task<Paged<T>> GetAllPaged(int page, int pageSize, CancellationToken cancellationToken);
    Task<List<T>> GetByIds(List<Guid> ids, CancellationToken cancellationToken);
    Task<T?> GetBy(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken, params Expression<Func<T, object>>[] includes);
    Task<T> Create(T entity, CancellationToken cancellationToken);
    Task<T> Update(T entity, CancellationToken cancellationToken);
    Task Delete(T entity, CancellationToken cancellationToken);
}
