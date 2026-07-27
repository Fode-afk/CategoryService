using CategoryService.Domain.Primitives;

namespace CategoryService.Domain.DomainEvents;

public sealed record CategoryDeletedDomainEvent(Guid CategoryId) : IDomainEvent;