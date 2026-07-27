using CategoryService.Domain.Primitives;

namespace CategoryService.Domain.DomainEvents;

public sealed record CategoryActivatedDomainEvent(
    Guid CategoryId,
    bool IsActive,
    long Version) : IDomainEvent;