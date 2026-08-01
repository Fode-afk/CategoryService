using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using ZiggyCreatures.Caching.Fusion;

namespace CategoryService.Application.Caching;

public sealed class InvalidateCacheHandlers(
    IFusionCache cache,
    ICategoryMetrics metrics) :
    IPostCommitDomainEventHandler<CategoryCreatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryInfoUpdatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryMovedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryReorderedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryActivatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryDeactivatedDomainEvent>,
    IPostCommitDomainEventHandler<CategoryDeletedDomainEvent>
{
    public async Task Handle(CategoryCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    public async Task Handle(CategoryInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    public async Task Handle(CategoryMovedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    public async Task Handle(CategoryReorderedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    public async Task Handle(CategoryActivatedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    public async Task Handle(CategoryDeactivatedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    public async Task Handle(CategoryDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        await RemoveBase(cancellationToken);

    private async Task RemoveBase(CancellationToken cancellationToken = default)
    {
        await cache.RemoveByTagAsync(CacheTags.Categories(), token: cancellationToken);

        metrics.RecordCacheInvalidation("categories:tree");
    }
}
