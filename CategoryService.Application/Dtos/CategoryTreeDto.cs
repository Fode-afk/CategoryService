namespace CategoryService.Application.Dtos;

public sealed record CategoryTreeDto(
    Guid CategoryId,
    string Name,
    string Slug,
    string? Image,
    int SortOrder)
{
    public List<CategoryTreeDto> Childrens { get; init; } = [];
}
