using CategoryService.Api.Grpc.V1.Protos;
using CategoryService.Application.Features.Commands.CreateCategory;
using CategoryService.Application.Features.Commands.UpdateCategoryInfo;
using CategoryService.Application.Features.Queries.SearchCategories;

namespace CategoryService.Api.Grpc.Mapping;

public static class CategoryGrpcMapper
{
    public static CreateCategoryCommand ToCreateCommand(this CreateCategoryRequest request) =>
        new(
            ParentCategoryId: request.HasParentCategoryId ? Guid.Parse(request.ParentCategoryId) : null,
            IsActive: request.IsActive,
            Name: request.Name,
            Slug: request.Slug,
            SeoTitle: request.SeoTitle,
            SeoDescription: request.SeoDescription,
            SeoKeywords: request.SeoKeywords,
            ImageUrl: request.ImageUrl);

    public static UpdateCategoryInfoCommand ToUpdateInfoCommand(this UpdateCategoryInfoRequest request) =>
         new(
            CategoryId: Guid.Parse(request.CategoryId),
            Name: request.Name,
            Slug: request.Slug,
            SeoTitle: request.SeoTitle,
            SeoDescription: request.SeoDescription,
            SeoKeywords: request.SeoKeywords,
            ImageUrl: request.ImageUrl);

    public static SearchCategoriesQuery ToSearchCategoriesQuery(this SearchCategoriesRequest request) =>
        request.HasMaxResults
            ? new SearchCategoriesQuery(request.Query, request.MaxResults)
            : new SearchCategoriesQuery(request.Query);

    public static CategoryTreeDto ToGrpc(this Application.Dtos.CategoryTreeDto dto)
    {
        var result = new CategoryTreeDto
        {
            CategoryId = dto.CategoryId.ToString(),
            Name = dto.Name,
            Slug = dto.Slug,
            Image = dto.Image ?? string.Empty,
            SortOrder = dto.SortOrder,
        };

        result.Childrens.AddRange(dto.Childrens.Select(ToGrpc));

        return result;
    }

    public static CategorySearchResultDto ToGrpc(this Application.Dtos.CategorySearchResultDto dto)
    {
        var result = new CategorySearchResultDto
        {
            CategoryId = dto.CategoryId.ToString(),
            Name = dto.Name,
            Slug = dto.Slug,
            IsActive = dto.IsActive,
            BreadcrumbPath = { dto.BreadcrumbPath },
        };

        if (dto.ParentCategoryId is not null)
            result.ParentCategoryId = dto.ParentCategoryId.ToString();

        return result;
    }
}