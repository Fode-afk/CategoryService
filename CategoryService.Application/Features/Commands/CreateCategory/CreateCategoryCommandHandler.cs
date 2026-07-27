using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.Errors;
using CategoryService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    IAppDbContext context,
    ICategoryMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<CreateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var isParentActive = true;

        if (request.ParentCategoryId.HasValue)
        {
            var parent = await context.Categories
                .Where(c => c.Id == request.ParentCategoryId.Value)
                .Select(c => new { c.IsActive })
                .FirstOrDefaultAsync(cancellationToken);

            if (parent is null)
                return Fail(CategoryErrors.ParentNotFound());

            isParentActive = parent.IsActive;
        }

        var maxSortOrder = await context.Categories
            .Where(c => c.ParentCategoryId == request.ParentCategoryId)
            .Select(c => (int?)c.SortOrder)
            .MaxAsync(cancellationToken);

        var sortOrder = (maxSortOrder ?? -1) + 1;

        var dataResult = CategoryCreationDataBuilder.Build(request);

        if (dataResult.IsFailure)
            return dataResult;

        var categoryResult = Category.Create(
            dataResult.Value,
            request.ParentCategoryId,
            sortOrder,
            request.IsActive && isParentActive,
            timeProvider.GetUtcNow());

        if (categoryResult.IsFailure)
            return categoryResult;

        context.Categories.Add(categoryResult.Value);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Fail(CategoryErrors.AlreadyExists());
        }

        metrics.RecordCategoryCreated();

        return Ok();
    }
}