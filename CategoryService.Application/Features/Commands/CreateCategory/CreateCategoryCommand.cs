using MediatR;
using migApp.Shared.Results;

namespace CategoryService.Application.Features.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    Guid? ParentCategoryId,
    bool IsActive,
    string Name, 
    string Slug, 
    string SeoTitle, 
    string SeoDescription, 
    string SeoKeywords,
    string Url) : IRequest<IResult>;