using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.MoveCategoryTo;

public sealed record MoveCategoryToCommand(
    Guid CategoryId,
    Guid? NewParentCategoryId) : IRequest<IResult>;