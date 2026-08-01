using CategoryService.Application.Dtos;
using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Queries.SearchCategories;

public sealed record SearchCategoriesQuery(
    string Query,
    int MaxResults = 20) : IRequest<IResult<IReadOnlyList<CategorySearchResultDto>>>;
