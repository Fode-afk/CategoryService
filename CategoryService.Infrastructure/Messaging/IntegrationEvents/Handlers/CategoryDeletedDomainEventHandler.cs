using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Categories;

namespace CategoryService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class CategoryDeletedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<CategoryDeletedDomainEvent>
{
    public async Task Handle(CategoryDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new CategoryDeletedIntegrationEvent(notification.CategoryId), cancellationToken);
}
