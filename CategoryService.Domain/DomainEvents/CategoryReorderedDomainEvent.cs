using CategoryService.Domain.Primitives;

namespace CategoryService.Domain.DomainEvents;

public sealed record CategoryReorderedDomainEvent() : IDomainEvent;