using CategoryService.Domain.DomainEvents;
using CategoryService.Domain.Primitives;
using MassTransit;
using migApp.Shared.Messaging.IntegrationEvents.Categories;

namespace CategoryService.Infrastructure.Messaging.IntegrationEvents.Handlers;

public sealed class CategoryCreatedDomainEventHandler(IPublishEndpoint publish) : IPreCommitDomainEventHandler<CategoryCreatedDomainEvent>
{
    public async Task Handle(CategoryCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await publish.Publish(new CategoryCreatedIntegrationEvent(
            notification.CategoryId,
            notification.Name,
            notification.Slug,
            notification.IsActive,
            notification.Version), cancellationToken);
}
