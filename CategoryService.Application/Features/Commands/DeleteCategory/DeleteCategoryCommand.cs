using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<IResult>;