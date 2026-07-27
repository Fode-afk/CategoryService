using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Commands.ReorderCategory;

public sealed class ReorderCategoryCommandHandler(
    IAppDbContext context,
    ICategoryMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<ReorderCategoryCommand, IResult>
{
    public async Task<IResult> Handle(ReorderCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
            return Fail(CategoryErrors.NotFound());

        var oldPosition = category.SortOrder;
        var newPosition = request.NewPosition;

        if (oldPosition == newPosition)
            return Ok();

        var siblings = await context.Categories
            .Where(c => c.ParentCategoryId == category.ParentCategoryId && c.Id != category.Id)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        var maxPosition = siblings.Count;
        if (newPosition < 0 || newPosition > maxPosition)
            return Fail(CategoryErrors.InvalidSortOrder());

        var now = timeProvider.GetUtcNow();

        if (newPosition < oldPosition)
        {
            foreach (var sibling in siblings.Where(s => s.SortOrder >= newPosition && s.SortOrder < oldPosition))
            {
                var shiftResult = sibling.Reorder(sibling.SortOrder + 1, now);
                if (shiftResult.IsFailure)
                    return shiftResult;
            }
        }
        else
        {
            foreach (var sibling in siblings.Where(s => s.SortOrder > oldPosition && s.SortOrder <= newPosition))
            {
                var shiftResult = sibling.Reorder(sibling.SortOrder - 1, now);
                if (shiftResult.IsFailure)
                    return shiftResult;
            }
        }

        var result = category.Reorder(newPosition, now);
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        metrics.RecordCategoryReordered();

        return Ok();
    }
}
