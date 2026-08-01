using CategoryService.Domain.Errors;
using CategoryService.Domain.ValueObjects;
using FluentValidation;

namespace CategoryService.Application.Features.Commands.UpdateCategoryInfo;

public sealed class UpdateCategoryInfoCommandValidator : AbstractValidator<UpdateCategoryInfoCommand>
{
    public UpdateCategoryInfoCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(CategoryNameErrorCodes.NullOrEmpty)
            .MaximumLength(CategoryName.MaxLength).WithErrorCode(CategoryNameErrorCodes.TooLong);

        RuleFor(x => x.Slug)
            .NotEmpty().WithErrorCode(SlugErrorCodes.NullOrEmpty)
            .MaximumLength(Slug.MaxLength).WithErrorCode(SlugErrorCodes.TooLong);

        RuleFor(x => x.SeoTitle)
            .NotEmpty().WithErrorCode(SeoTitleErrorCodes.NullOrEmpty)
            .MaximumLength(SeoTitle.MaxLength).WithErrorCode(SeoTitleErrorCodes.TooLong);

        RuleFor(x => x.SeoDescription)
            .NotEmpty().WithErrorCode(SeoDescriptionErrorCodes.NullOrEmpty)
            .MaximumLength(SeoDescription.MaxLength).WithErrorCode(SeoDescriptionErrorCodes.TooLong);

        RuleFor(x => x.SeoKeywords)
            .NotEmpty().WithErrorCode(SeoKeywordsErrorCodes.NullOrEmpty)
            .MaximumLength(SeoKeywords.MaxLength).WithErrorCode(SeoKeywordsErrorCodes.TooLong);

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithErrorCode(ImageUrlErrorCodes.NullOrEmpty)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithErrorCode(ImageUrlErrorCodes.InvalidFormat);
    }
}