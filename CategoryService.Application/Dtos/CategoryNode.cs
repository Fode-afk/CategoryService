namespace CategoryService.Application.Dtos;

internal sealed record CategoryNode(
    Guid CategoryId,
    Guid? ParentCategoryId,
    string Name,
    string Slug,
    string? Image,
    int SortOrder);
