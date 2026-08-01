namespace CategoryService.Infrastructure.BackgroundServices;

public sealed class SoftDeletedCategoriesCleanupOptions
{
    public const string SectionName = "SoftDeletedCategoriesCleanup";

    public TimeSpan Interval { get; init; } = TimeSpan.FromHours(6);
    public TimeSpan RetentionPeriod { get; init; } = TimeSpan.FromDays(30);
    public int BatchSize { get; init; } = 500;
}
