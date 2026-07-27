using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Categories;

namespace CategoryService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class CategoryActivatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<CategoryActivatedDomainEvent>
{
    public async Task Handle(CategoryActivatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new CategoryActivatedIntegrationEvent(
            notification.CategoryId,
            notification.IsActive,
            notification.Version), cancellationToken);
}
