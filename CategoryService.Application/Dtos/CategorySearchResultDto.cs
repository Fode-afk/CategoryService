namespace CategoryService.Application.Dtos;

public sealed record CategorySearchResultDto(
    Guid CategoryId,
    string Name,
    string Slug,
    bool IsActive,
    Guid? ParentCategoryId,
    IReadOnlyList<string> BreadcrumbPath);
