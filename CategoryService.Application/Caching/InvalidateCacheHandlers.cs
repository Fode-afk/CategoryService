using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using ZiggyCreatures.Caching.Fusion;

namespace CategoryService.Application.Caching;

public sealed class InvalidateCacheHandlers(IFusionCache cache) :
    IPostCommitDomainEventHandler<CategoryCreatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryInfoUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryMovedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryReorderedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryActivatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryDeactivatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryDeletedDomainEvent>
{
    public async Task Handle(CategoryCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);

    public async Task Handle(CategoryInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);

    public async Task Handle(CategoryMovedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);

    public async Task Handle(CategoryReorderedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);

    public async Task Handle(CategoryActivatedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);

    public async Task Handle(CategoryDeactivatedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);

    public async Task Handle(CategoryDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        await cache.RemoveAsync(CacheKeys.Categories(), token: cancellationToken);
}
