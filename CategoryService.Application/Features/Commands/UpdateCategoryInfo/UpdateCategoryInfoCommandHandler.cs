using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Commands.UpdateCategoryInfo;

public sealed class UpdateCategoryInfoCommandHandler(
    IAppDbContext context,
    ICategoryMetrics metrics,
    TimeProvider timeProvider) : IRequestHandler<UpdateCategoryInfoCommand, IResult>
{
    public async Task<IResult> Handle(UpdateCategoryInfoCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
            return Fail(CategoryErrors.NotFound());

        var dataResult = CategoryUpdateInfoDataBuilder.Build(request);

        if (dataResult.IsFailure)
            return dataResult;

        var result = category.UpdateInfo(
            dataResult.Value, 
            timeProvider.GetUtcNow());

        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        metrics.RecordCategoryUpdated();

        return Ok();
    }
}