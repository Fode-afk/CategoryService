using CategoryService.Domain.Primitives;

namespace CategoryService.Domain.DomainEvents;

public sealed record CategoryDeactivatedDomainEvent(
    Guid CategoryId,
    bool IsActive,
    long Version) : IDomainEvent;