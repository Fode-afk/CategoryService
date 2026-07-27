using CategoryService.Domain.Primitives;
using CategoryService.Domain.ValueObjects;

namespace CategoryService.Domain.DomainEvents;

public sealed record CategoryCreatedDomainEvent(
    Guid CategoryId,
    CategoryName Name,
    Slug Slug,
    bool IsActive,
    long Version) : IDomainEvent;