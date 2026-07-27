using CategoryService.Infrastructure.Data.Seeds;

namespace CategoryService.Infrastructure.Data;

internal static class DbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Categories.Any())
            context.AddRange(CategorySeed.Data);

        await context.SaveChangesAsync();
    }
}