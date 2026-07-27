using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Categories;

namespace CategoryService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class CategoryInfoUpdatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<CategoryInfoUpdatedDomainEvent>
{
    public async Task Handle(CategoryInfoUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new CategoryInfoUpdatedIntegrationEvent(
            notification.CategoryId,
            notification.Name,
            notification.Slug,
            notification.Version), cancellationToken);
}
