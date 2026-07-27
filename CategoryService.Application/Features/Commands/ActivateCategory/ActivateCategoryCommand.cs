using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.ActivateCategory;

public sealed record ActivateCategoryCommand(Guid CategoryId) : IRequest<IResult>;