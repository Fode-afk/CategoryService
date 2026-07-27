using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class SlugErrors 
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(SlugErrorCodes.NullOrEmpty,
            "Slug is null or empty.");

    public static Error InvalidFormat() =>
        Error.InvalidArgument(SlugErrorCodes.InvalidFormat,
            "Slug has an invalid format.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(SlugErrorCodes.TooLong,
            $"Slug is too long. Maximum length is {maxLength} characters.");

    public static Error TooShort(int minLength) =>
        Error.InvalidArgument(SlugErrorCodes.TooShort,
            $"Slug is too short. Minimum length is {minLength} characters.");

    public static Error NotUnique() =>
        Error.InvalidArgument(SlugErrorCodes.NotUnique,
            "Slug must be unique.");
}

public static class SlugErrorCodes
{
    public const string NullOrEmpty = "Slug.NullOrEmpty";
    public const string InvalidFormat = "Slug.InvalidFormat";
    public const string TooLong = "Slug.TooLong";
    public const string TooShort = "Slug.TooShort";
    public const string NotUnique = "Slug.NotUnique";
}
