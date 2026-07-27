using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Categories;

namespace CategoryService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class CategoryDeactivatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<CategoryDeactivatedDomainEvent>
{
    public async Task Handle(CategoryDeactivatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new CategoryDeactivatedIntegrationEvent(
            notification.CategoryId,
            notification.IsActive,
            notification.Version), cancellationToken);
}
