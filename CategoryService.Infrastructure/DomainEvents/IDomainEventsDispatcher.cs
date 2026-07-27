using CategoryService.Domain.Primitives;

namespace CategoryService.Infrastructure.DomainEvents;

public interface IDomainEventsDispatcher
{
    Task DispatchPreCommitDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    Task DispatchPostCommitDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
