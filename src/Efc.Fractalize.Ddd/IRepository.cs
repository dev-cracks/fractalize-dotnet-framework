namespace Efc.Fractalize.Ddd;

/// <summary>
/// Marker for the repository of an aggregate root: aggregates are loaded and saved as a whole through it.
/// </summary>
/// <typeparam name="TEntity">Type of the aggregate root.</typeparam>
/// <typeparam name="Tid">Type of the identifier of the aggregate root.</typeparam>
public interface IRepository<TEntity, Tid> where TEntity : AggregateRootBase<Tid>
{
}
