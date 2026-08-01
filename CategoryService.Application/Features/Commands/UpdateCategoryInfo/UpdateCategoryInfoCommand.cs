using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.UpdateCategoryInfo;

public sealed record UpdateCategoryInfoCommand(
    Guid CategoryId,
    string Name,
    string Slug,
    string SeoTitle,
    string SeoDescription,
    string SeoKeywords,
    string ImageUrl) : IRequest<IResult>;