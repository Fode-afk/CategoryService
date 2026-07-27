using CategoryService.Domain.ValueObjects;

namespace CategoryService.Domain.RequestData;

public sealed record CategoryUpdateInfoData(
    CategoryName Name,
    Slug Slug,
    SeoMetadata SeoMetadata,
    ImageUrl? Image);