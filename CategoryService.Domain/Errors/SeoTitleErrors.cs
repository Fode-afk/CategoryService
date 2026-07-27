using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class SeoTitleErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(SeoTitleErrorCodes.NullOrEmpty,
            "SEO title is null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(SeoTitleErrorCodes.TooLong,
            $"SEO title is too long. Maximum length is {maxLength} characters.");
}

public static class SeoTitleErrorCodes
{
    public const string NullOrEmpty = "SeoTitle.NullOrEmpty";
    public const string TooLong = "SeoTitle.TooLong";
}