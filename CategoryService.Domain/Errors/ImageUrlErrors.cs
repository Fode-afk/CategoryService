using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class ImageUrlErrors
{
    public static Error NullOrEmpty() =>
        Error.InvalidArgument(ImageUrlErrorCodes.NullOrEmpty,
            "Image URL cannot be null or empty.");

    public static Error InvalidFormat() =>
        Error.InvalidArgument(ImageUrlErrorCodes.InvalidFormat,
            "Image URL format is invalid.");
}

public static class ImageUrlErrorCodes
{
    public const string NullOrEmpty = "ImageUrl.NullOrEmpty";
    public const string InvalidFormat = "ImageUrl.InvalidFormat";
}
