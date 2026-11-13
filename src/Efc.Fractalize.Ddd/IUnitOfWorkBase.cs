namespace Efc.Fractalize.Ddd;

public interface IUnitOfWorkBase : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
