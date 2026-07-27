using CategoryService.Domain.Models;
using CategoryService.Domain.RequestData;
using CategoryService.Domain.ValueObjects;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CategoryService.Infrastructure.Data.Seeds;

internal static partial class CategorySeed
{
    internal sealed class WbCategoryRawDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string? Seo { get; set; }
        public List<WbCategoryRawDto>? Nodes { get; set; }
        public List<WbCategoryRawDto>? Childs { get; set; }

        public IReadOnlyList<WbCategoryRawDto> Children =>
            (IReadOnlyList<WbCategoryRawDto>?)Nodes ?? Childs ?? [];
    }

    internal sealed class WbCategoryRootDto
    {
        public List<WbCategoryRawDto> Data { get; set; } = [];
    }

    internal sealed class MergedCategoryNode
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
        public string? Seo { get; set; }
        public int? ParentId { get; set; }
        public List<int> ChildrenIds { get; } = [];
    }

    public static readonly List<Category> Data = Build();

    private static List<Category> Build()
    {
        var root = LoadJson();

        var nodesById = new Dictionary<int, MergedCategoryNode>();
        var rootOrder = new List<int>();

        foreach (var node in root.Data)
            Merge(node, parentId: null, nodesById, rootOrder);

        var now = DateTimeOffset.UtcNow;
        var result = new List<Category>();
        var usedSlugs = new HashSet<(Guid? Parent, string Slug)>();

        var sortCounters = new Dictionary<Guid, int>();

        foreach (var rootId in rootOrder)
            Visit(rootId, parentId: null, nodesById, sortCounters, now, result, usedSlugs);

        return result;
    }

    private static void Merge(
        WbCategoryRawDto raw,
        int? parentId,
        Dictionary<int, MergedCategoryNode> nodesById,
        List<int> rootOrder)
    {
        if (!nodesById.TryGetValue(raw.Id, out var merged))
        {
            merged = new MergedCategoryNode { Name = raw.Name, Url = raw.Url, Seo = raw.Seo, ParentId = parentId };
            nodesById[raw.Id] = merged;

            if (parentId is null)
                rootOrder.Add(raw.Id);
        }
        else if (merged.Seo is null && raw.Seo is not null)
        {
            merged.Seo = raw.Seo;
        }

        foreach (var child in raw.Children)
        {
            if (!merged.ChildrenIds.Contains(child.Id))
                merged.ChildrenIds.Add(child.Id);

            Merge(child, raw.Id, nodesById, rootOrder);
        }
    }

    private static void Visit(
    int wbId,
    Guid? parentId,
    Dictionary<int, MergedCategoryNode> nodesById,
    Dictionary<Guid, int> sortCounters,
    DateTimeOffset now,
    List<Category> result,
    HashSet<(Guid? Parent, string Slug)> usedSlugs)
    {
        var node = nodesById[wbId];
        var id = ToGuid(wbId);

        var counterKey = parentId ?? Guid.Empty;
        var sortOrder = sortCounters.GetValueOrDefault(counterKey);
        sortCounters[counterKey] = sortOrder + 1;

        var nameResult = CategoryName.Create(node.Name);
        if (nameResult.IsFailure)
            throw new InvalidOperationException($"wbId={wbId} name invalid: {nameResult.Error.Message}");

        var slugValue = ResolveUniqueSlug(node.Url, node.Name, parentId, usedSlugs);
        var slugResult = Slug.Create(slugValue + Guid.NewGuid().ToString());
        if (slugResult.IsFailure)
            throw new InvalidOperationException($"wbId={wbId} slug invalid: {slugResult.Error.Message}");

        var seoTitleResult = SeoTitle.Create(node.Seo ?? node.Name);
        if (seoTitleResult.IsFailure)
            throw new InvalidOperationException($"wbId={wbId} seoTitle invalid: {seoTitleResult.Error.Message}");

        var seoMetadataResult = SeoMetadata.Create(seoTitleResult.Value, SeoDescription.Create("Null").Value, SeoKeywords.Create("Null").Value);
        if (seoMetadataResult.IsFailure)
            throw new InvalidOperationException($"wbId={wbId} seoMetadata invalid: {seoMetadataResult.Error.Message}");

        var categoryResult = Category.CreateWithId(
            id,
            new CategoryCreationData(nameResult.Value, slugResult.Value, seoMetadataResult.Value, null),
            parentId,
            sortOrder,
            isActive: true,
            now);

        if (categoryResult.IsFailure)
            throw new InvalidOperationException($"wbId={wbId} create failed: {categoryResult.Error.Message}");

        var category = categoryResult.Value;
        category.ClearDomainEvents();
        result.Add(category);

        foreach (var childId in node.ChildrenIds)
            Visit(childId, id, nodesById, sortCounters, now, result, usedSlugs);
    }

    private static string ResolveUniqueSlug(
    string url, string name, Guid? parentId, HashSet<(Guid? Parent, string Slug)> usedSlugs)
    {
        var rawSegment = url.TrimEnd('/').Split('/').Last();
        var baseSlug = Normalize(rawSegment);

        if (baseSlug.Length < Slug.MinLength)
            baseSlug = Normalize(Transliterate(name));

        if (baseSlug.Length < Slug.MinLength)
            baseSlug = baseSlug.PadRight(Slug.MinLength, '0');

        var slug = baseSlug;
        var suffix = 2;

        while (!usedSlugs.Add((parentId, slug)))
            slug = $"{baseSlug}-{suffix++}";

        return slug;
    }

    private static string Transliterate(string value)
    {
        var map = new Dictionary<char, string>
        {
            ['а'] = "a",
            ['б'] = "b",
            ['в'] = "v",
            ['г'] = "g",
            ['д'] = "d",
            ['е'] = "e",
            ['ё'] = "e",
            ['ж'] = "zh",
            ['з'] = "z",
            ['и'] = "i",
            ['й'] = "y",
            ['к'] = "k",
            ['л'] = "l",
            ['м'] = "m",
            ['н'] = "n",
            ['о'] = "o",
            ['п'] = "p",
            ['р'] = "r",
            ['с'] = "s",
            ['т'] = "t",
            ['у'] = "u",
            ['ф'] = "f",
            ['х'] = "h",
            ['ц'] = "ts",
            ['ч'] = "ch",
            ['ш'] = "sh",
            ['щ'] = "sch",
            ['ъ'] = "",
            ['ы'] = "y",
            ['ь'] = "",
            ['э'] = "e",
            ['ю'] = "yu",
            ['я'] = "ya"
        };

        var result = new StringBuilder();
        foreach (var c in value.ToLowerInvariant())
            result.Append(map.TryGetValue(c, out var mapped) ? mapped : c.ToString());

        return result.ToString();
    }

    public static string Normalize(string rawSegment)
    {
        var value = rawSegment.ToLowerInvariant();
        value = InvalidCharsRegex().Replace(value, "-");
        value = MultipleDashesRegex().Replace(value, "-");
        return value.Trim('-');
    }

    [GeneratedRegex(@"[^a-z0-9-]+")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex MultipleDashesRegex();

    private static Guid ToGuid(int wbId)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(wbId).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

    private static WbCategoryRootDto LoadJson()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith("categories.json"));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
        var json = reader.ReadToEnd();

        return JsonSerializer.Deserialize<WbCategoryRootDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Deserialized to null.");
    }
}