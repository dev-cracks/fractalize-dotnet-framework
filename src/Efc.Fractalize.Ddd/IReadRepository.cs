using System.Linq.Expressions;

namespace Efc.Fractalize.Ddd;

/// <summary>
///
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="Tid"></typeparam>
public interface IReadRepository<TEntity, Tid> where TEntity : EntityBase<Tid>
{
    public Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    public Task<TEntity> GetByIdAsync(Tid id, CancellationToken cancellationToken = default);
}
