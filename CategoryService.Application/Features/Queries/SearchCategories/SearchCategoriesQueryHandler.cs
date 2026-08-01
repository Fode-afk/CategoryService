using CategoryService.Application.Dtos;
using CategoryService.Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Queries.SearchCategories;

public sealed class SearchCategoriesQueryHandler(IAppDbContext context)
    : IRequestHandler<SearchCategoriesQuery, IResult<IReadOnlyList<CategorySearchResultDto>>>
{
    private sealed record CategoryPathNode(string Name, Guid? ParentCategoryId);

    public async Task<IResult<IReadOnlyList<CategorySearchResultDto>>> Handle(
        SearchCategoriesQuery request, CancellationToken cancellationToken)
    {
        var trimmedQuery = request.Query.Trim();

        var matches = await context.Categories
            .AsNoTracking()
            .Where(c => c.Name.Value.Contains(trimmedQuery))
            .OrderBy(c => c.Name.Value)
            .Take(request.MaxResults)
            .Select(c => new
            {
                c.Id,
                Name = c.Name.Value,
                Slug = c.Slug.Value,
                c.IsActive,
                c.ParentCategoryId
            })
            .ToListAsync(cancellationToken);

        if (matches.Count == 0)
            return Ok<IReadOnlyList<CategorySearchResultDto>>([]);

        var allForPathResolution = await context.Categories
            .AsNoTracking()
            .Select(c => new CategoryPathNode(c.Name.Value, c.ParentCategoryId))
            .ToDictionaryAsync(_ => Guid.Empty, cancellationToken);

        var nodesById = await context.Categories
            .AsNoTracking()
            .ToDictionaryAsync(
                c => c.Id,
                c => new CategoryPathNode(c.Name.Value, c.ParentCategoryId),
                cancellationToken);

        var results = matches
            .Select(m => new CategorySearchResultDto(
                m.Id,
                m.Name,
                m.Slug,
                m.IsActive,
                m.ParentCategoryId,
                BuildBreadcrumbPath(m.ParentCategoryId, nodesById)))
            .ToList();

        return Ok<IReadOnlyList<CategorySearchResultDto>>(results);
    }

    private static List<string> BuildBreadcrumbPath(
        Guid? parentId,
        Dictionary<Guid, CategoryPathNode> nodesById)
    {
        var path = new List<string>();
        var currentId = parentId;

        while (currentId.HasValue && nodesById.TryGetValue(currentId.Value, out var node))
        {
            path.Insert(0, node.Name);
            currentId = node.ParentCategoryId;
        }

        return path;
    }
}
