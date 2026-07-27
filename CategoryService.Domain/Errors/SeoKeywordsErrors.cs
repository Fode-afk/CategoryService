using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class SeoKeywordsErrors
{
    public static Error NullOrEmpty() => 
        Error.InvalidArgument(SeoKeywordsErrorCodes.NullOrEmpty,
            "SEO keywords are null or empty.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(SeoKeywordsErrorCodes.TooLong,
            $"SEO keywords are too long. Maximum length is {maxLength} characters.");
}

public static class SeoKeywordsErrorCodes
{
    public const string NullOrEmpty = "SeoKeywords.NullOrEmpty";
    public const string TooLong = "SeoKeywords.TooLong";
}