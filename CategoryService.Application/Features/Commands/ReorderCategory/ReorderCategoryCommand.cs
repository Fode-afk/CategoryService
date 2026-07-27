using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.ReorderCategory;

public sealed record ReorderCategoryCommand(
    Guid CategoryId,
    int NewPosition) : IRequest<IResult>;