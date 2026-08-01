using CategoryService.Application.Caching;
using CategoryService.Application.Dtos;
using CategoryService.Application.Interfaces.Data;
using CategoryService.Application.Interfaces.Metrics;
using CategoryService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Queries.GetAllCategoriesAdmin;

public sealed class GetAllCategoriesAdminQueryHandler(
    IAppDbContext context,
    ICategoryMetrics metrics,
    IFusionCache cache) : IRequestHandler<GetAllCategoriesAdminQuery, IResult<IReadOnlyList<CategoryTreeDto>>>
{
    public async Task<IResult<IReadOnlyList<CategoryTreeDto>>> Handle(GetAllCategoriesAdminQuery request, CancellationToken cancellationToken)
    {
        var wasHit = true;

        var tree = await cache.GetOrSetAsync<IReadOnlyList<CategoryTreeDto>?>(
            CacheKeys.CategoriesAdmin(),
            async (entry, ct) =>
            {
                wasHit = false;

                var categories = await context.Categories
                    .AsNoTracking()
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new CategoryNode(
                        x.Id,
                        x.ParentCategoryId,
                        x.Name.Value,
                        x.Slug.Value,
                        x.Image != null ? x.Image.Value : null,
                        x.SortOrder))
                    .ToListAsync(ct);

                if (categories is null)
                    return null;

                return BuildTree(categories);
            },
            tags: [CacheTags.Categories()],
            token: cancellationToken);

        metrics.RecordCacheHitOrMiss("categories-admin-tree", wasHit);

        return tree == null ?
            Fail<IReadOnlyList<CategoryTreeDto>>(CategoryErrors.NotFound()) :
            Ok(tree);
    }

    private static List<CategoryTreeDto> BuildTree(List<CategoryNode> nodes)
    {
        var lookup = nodes.ToDictionary(
            x => x.CategoryId,
            x => new CategoryTreeDto(
                x.CategoryId,
                x.Name,
                x.Slug,
                x.Image,
                x.SortOrder));

        var roots = new List<CategoryTreeDto>();

        foreach (var node in nodes)
        {
            if (node.ParentCategoryId is null)
            {
                roots.Add(lookup[node.CategoryId]);
                continue;
            }

            lookup[node.ParentCategoryId.Value]
                .Childrens
                .Add(lookup[node.CategoryId]);
        }

        return roots;
    }
}
