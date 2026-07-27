using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CategoryService.Infrastructure.Observability;

public sealed class ActiveCategoriesMetricCollector(
    IServiceScopeFactory scopeFactory,
    ICategoryMetrics metrics,
    ILogger<ActiveCategoriesMetricCollector> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider
                    .GetRequiredService<IAppDbContext>();

                var count = await context.Categories
                    .CountAsync(c => c.IsActive, cancellationToken);

                metrics.SetActiveCategoriesCount(count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to collect active categories count");
            }
        }
    }
}