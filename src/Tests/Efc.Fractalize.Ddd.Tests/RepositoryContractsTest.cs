using System.Linq.Expressions;

namespace Efc.Fractalize.Ddd.Tests;

public class RepositoryContractsTest
{
    private static readonly PersonStub[] People =
    [
        new() { Id = Guid.NewGuid(), Name = "Charles", IsActive = true },
        new() { Id = Guid.NewGuid(), Name = "Ana", IsActive = false },
        new() { Id = Guid.NewGuid(), Name = "Martha", IsActive = true },
    ];

    [Fact]
    public async Task WhereAsync_WithAnExpression_ReturnsTheMatchingEntities()
    {
        IReadRepository<PersonStub, Guid> repository = new InMemoryReadRepository(People);

        var active = await repository.WhereAsync(person => person.IsActive);

        Assert.Equal(["Charles", "Martha"], active.Select(person => person.Name));
    }

    [Fact]
    public async Task GetAllAsyncAndGetByIdAsync_ReturnTheStoredEntities()
    {
        IReadRepository<PersonStub, Guid> repository = new InMemoryReadRepository(People);

        Assert.Equal(3, (await repository.GetAllAsync()).Count);
        Assert.Same(People[1], await repository.GetByIdAsync(People[1].Id));
    }

    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_ReturnsTheNumberOfChanges()
    {
        using IUnitOfWorkBase unitOfWork = new CountingUnitOfWork();

        Assert.Equal(1, await unitOfWork.SaveChangesAsync());
    }

    [Fact]
    public void Repository_IsAMarkerForAggregateRoots() =>
        Assert.Empty(typeof(IRepository<OrderStub, Guid>).GetMembers());

    private sealed class PersonStub : EntityBase<Guid>
    {
        public required string Name { get; init; }

        public bool IsActive { get; init; }
    }

    private sealed class OrderStub : AggregateRootBase<Guid>;

    private sealed class InMemoryReadRepository(IReadOnlyCollection<PersonStub> people) : IReadRepository<PersonStub, Guid>
    {
        public Task<IReadOnlyCollection<PersonStub>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(people);

        public Task<IReadOnlyCollection<PersonStub>> WhereAsync(
            Expression<Func<PersonStub, bool>> expression, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<PersonStub>>([.. people.Where(expression.Compile())]);

        public Task<PersonStub> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(people.Single(person => person.Id == id));
    }

    private sealed class CountingUnitOfWork : IUnitOfWorkBase
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

        public void Dispose()
        {
        }
    }
}
