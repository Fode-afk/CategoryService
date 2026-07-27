using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class CategoryNameErrors
{
    public static Error NullOrEmpty() => 
        Error.InvalidArgument(CategoryNameErrorCodes.NullOrEmpty,
            "Category name cannot be null or empty.");

    public static Error TooShort(int minLength) => 
        Error.InvalidArgument(CategoryNameErrorCodes.TooShort,
            $"Category name must be at least {minLength} characters long.");

    public static Error TooLong(int maxLength) =>
        Error.InvalidArgument(CategoryNameErrorCodes.TooLong,
            $"Category name must not exceed {maxLength} characters.");
}

public static class CategoryNameErrorCodes
{
    public const string NullOrEmpty = "CategoryName.NullOrEmpty";
    public const string TooShort = "CategoryName.TooShort";
    public const string TooLong = "CategoryName.TooLong";
}