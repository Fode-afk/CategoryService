using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class SeoDescriptionErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(SeoDescriptionErrorCodes.NullOrEmpty,
            "SEO description is null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(SeoDescriptionErrorCodes.TooLong,
            $"SEO description is too long. Maximum length is {maxLength} characters.");
}

public static class SeoDescriptionErrorCodes
{
    public const string NullOrEmpty = "SeoDescription.NullOrEmpty";
    public const string TooLong = "SeoDescription.TooLong";
}