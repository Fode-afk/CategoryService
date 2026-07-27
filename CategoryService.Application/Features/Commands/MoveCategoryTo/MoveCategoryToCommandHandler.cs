using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Commands.MoveCategoryTo;

public sealed class MoveCategoryToCommandHandler(
    IAppDbContext context,
    ICategoryMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<MoveCategoryToCommand, IResult>
{
    public async Task<IResult> Handle(MoveCategoryToCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
            return Fail(CategoryErrors.NotFound());

        if (request.NewParentCategoryId.HasValue)
        {
            var newParentExists = await context.Categories
                .AnyAsync(c => c.Id == request.NewParentCategoryId.Value, cancellationToken);

            if (!newParentExists)
                return Fail(CategoryErrors.ParentNotFound());

            var wouldCreateCycle = await WouldCreateCycleAsync(
                category.Id, request.NewParentCategoryId.Value, cancellationToken);

            if (wouldCreateCycle)
                return Fail(CategoryErrors.CircularReference());
        }

        var result = category.MoveTo(request.NewParentCategoryId, timeProvider.GetUtcNow());

        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        metrics.RecordCategoryMoved();

        return Ok();
    }

    private async Task<bool> WouldCreateCycleAsync(
       Guid categoryId, Guid newParentCategoryId, CancellationToken cancellationToken)
    {
        var currentId = (Guid?)newParentCategoryId;

        while (currentId.HasValue)
        {
            if (currentId.Value == categoryId)
                return true;

            currentId = await context.Categories
                .Where(c => c.Id == currentId.Value)
                .Select(c => c.ParentCategoryId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return false;
    }
}
