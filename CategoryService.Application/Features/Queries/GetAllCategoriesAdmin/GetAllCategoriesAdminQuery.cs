using CategoryService.Application.Dtos;
using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Queries.GetAllCategoriesAdmin;

public sealed record GetAllCategoriesAdminQuery : IRequest<IResult<IReadOnlyList<CategoryTreeDto>>>;
