using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : IRequest<IResult>;