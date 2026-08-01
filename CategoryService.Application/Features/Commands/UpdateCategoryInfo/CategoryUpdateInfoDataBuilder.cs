using CategoryService.Domain.RequestData;
using CategoryService.Domain.ValueObjects;
using migApp.Shared.Domain.Errors;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Application.Features.Commands.UpdateCategoryInfo;

internal static class CategoryUpdateInfoDataBuilder
{
    public static IResult<CategoryUpdateInfoData> Build(UpdateCategoryInfoCommand request)
    {
        var errors = new List<Error>();

        var nameResult = CategoryName.Create(request.Name);
        if (nameResult.IsFailure)
            errors.Add(nameResult.Error);

        var slugResult = Slug.Create(request.Slug);
        if (slugResult.IsFailure)
            errors.Add(slugResult.Error);

        var seoTitleResult = SeoTitle.Create(request.SeoTitle);
        if (seoTitleResult.IsFailure)
            errors.Add(seoTitleResult.Error);

        var seoDescriptionResult = SeoDescription.Create(request.SeoDescription);
        if (seoDescriptionResult.IsFailure)
            errors.Add(seoDescriptionResult.Error);

        var seoKeywordsResult = SeoKeywords.Create(request.SeoKeywords);
        if (seoKeywordsResult.IsFailure)
            errors.Add(seoKeywordsResult.Error);

        SeoMetadata? seoMetadata = null;
        if (seoTitleResult.IsSuccess && seoDescriptionResult.IsSuccess && seoKeywordsResult.IsSuccess)
        {
            var seoMetadataResult = SeoMetadata.Create(
                seoTitleResult.Value,
                seoDescriptionResult.Value,
                seoKeywordsResult.Value);
            if (seoMetadataResult.IsFailure)
                errors.Add(seoMetadataResult.Error);
            else
                seoMetadata = seoMetadataResult.Value;
        }

        var imageUrlResult = ImageUrl.Create(request.ImageUrl);
        if (imageUrlResult.IsFailure)
            errors.Add(imageUrlResult.Error);

        if (errors.Count > 0)
            return Fail<CategoryUpdateInfoData>(Error.Validation(
                CommonErrorCodes.ValidationFailed,
                new Dictionary<string, object>
                {
                    ["Errors"] = errors.Select(e => e.Code).ToList()
                }));

        return Ok(
            new CategoryUpdateInfoData(
                nameResult.Value,
                slugResult.Value,
                seoMetadata!,
                imageUrlResult.Value));
    }
}
