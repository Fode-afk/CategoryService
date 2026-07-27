using migApp.Shared.Results;

namespace CategoryService.Domain.Errors;

public static class CategoryErrors
{
    public static Error CannotBeOwnParent() =>
        Error.InvalidArgument(CategoryErrorCodes.InvalidParentCategory, 
            "A category cannot be its own parent.");

    public static Error AlreadyExists() =>
        Error.InvalidArgument(CategoryErrorCodes.AlreadyExists,
            "A category with the same name already exists under the same parent.");

    public static Error ParentNotFound() =>
       Error.InvalidArgument(CategoryErrorCodes.ParentNotFound, 
           "Parent category was not found.");

    public static Error NotFound() =>
        Error.NotFound(CategoryErrorCodes.NotFound,
            "Category was not found.");

    public static Error InvalidSortOrder() =>
       Error.InvalidArgument(CategoryErrorCodes.InvalidSortOrder, 
           "The requested position is out of range.");

    public static Error CircularReference() =>
        Error.InvalidArgument(CategoryErrorCodes.CircularReference,
            "Moving the category to the specified parent would create a circular reference.");

    public static Error ParentNotActive() =>
        Error.InvalidArgument(CategoryErrorCodes.ParentNotActive,
            "Cannot activate a category while its parent is inactive.");
}

public static class CategoryErrorCodes
{
    public const string InvalidParentCategory = "Category.InvalidParentCategory";
    public const string AlreadyExists = "Category.AlreadyExists";
    public const string ParentNotFound = "Category.ParentNotFound";
    public const string NotFound = "Category.NotFound";
    public const string InvalidSortOrder = "Category.InvalidSortOrder";
    public const string CircularReference = "Category.CircularReference";
    public const string ParentNotActive = "Category.ParentNotActive";
}