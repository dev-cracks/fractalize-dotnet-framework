namespace Efc.Fractalize.Ddd;

public interface IRepository<TEntity, Tid> where TEntity : AggregateRootBase<Tid>
{
}
