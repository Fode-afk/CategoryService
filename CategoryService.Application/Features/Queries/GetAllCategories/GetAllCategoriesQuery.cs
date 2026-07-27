using CategoryService.Application.Dtos;
using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery : IRequest<IResult<IReadOnlyList<CategoryTreeDto>>>;