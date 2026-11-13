using System.Linq.Expressions;

namespace Efc.Fractalize.Ddd;

/// <summary>
/// Read-only access to the entities of one type. Implemented by the infrastructure layer.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="Tid">Type of the identifier of the entity.</typeparam>
public interface IReadRepository<TEntity, Tid> where TEntity : EntityBase<Tid>
{
    /// <summary>
    /// Returns every entity.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the entities that match an expression.
    /// </summary>
    /// <param name="expression">Filter to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public Task<IReadOnlyCollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the entity with an identifier.
    /// </summary>
    /// <param name="id">Identifier of the entity.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public Task<TEntity> GetByIdAsync(Tid id, CancellationToken cancellationToken = default);
}
