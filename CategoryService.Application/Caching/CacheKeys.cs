namespace CategoryService.Application.Caching;

public static class CacheKeys
{
    public static string Categories() => "categories:tree";
    public static string CategoriesAdmin() => "categories:admin";
}
