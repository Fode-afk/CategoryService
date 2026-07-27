using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.Errors;
using CategoryService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Commands.ActivateCategory;

public sealed class ActivateCategoryCommandHandler(
    IAppDbContext context,
    ICategoryMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<ActivateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(ActivateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
            return Fail(CategoryErrors.NotFound());

        if (category.ParentCategoryId.HasValue)
        {
            var parentIsActive = await context.Categories
                .Where(c => c.Id == category.ParentCategoryId.Value)
                .Select(c => c.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (!parentIsActive)
                return Fail(CategoryErrors.ParentNotActive());
        }

        var now = timeProvider.GetUtcNow();

        var result = category.Activate(now);
        if (result.IsFailure)
            return result;

        var all = await context.Categories.ToListAsync(cancellationToken);

        var descendants = new List<Category>();
        var queue = new Queue<Guid>();
        queue.Enqueue(category.Id);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = all.Where(c => c.ParentCategoryId == currentId).ToList();

            foreach (var child in children)
            {
                descendants.Add(child);
                queue.Enqueue(child.Id);
            }
        }

        foreach (var descendant in descendants)
        {
            var descendantResult = descendant.Activate(now);
            if (descendantResult.IsFailure)
                return descendantResult;
        }

        if (descendants.Count > 0)
            metrics.RecordCascadeActivation(descendants.Count);

        await context.SaveChangesAsync(cancellationToken);

        metrics.RecordCategoryActivated();

        return Ok();
    }
}