using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.ValueObjects;

public sealed class SeoMetadata : ValueObject
{
    public SeoTitle Title { get; }
    public SeoDescription Description { get; }
    public SeoKeywords Keywords { get; }

    private SeoMetadata(
        SeoTitle title,
        SeoDescription description,
        SeoKeywords keywords)
    {
        Title = title;
        Description = description;
        Keywords = keywords;
    }

    public static IResult<SeoMetadata> Create(
        SeoTitle title,
        SeoDescription description,
        SeoKeywords keywords)
    {      
        return Ok(new SeoMetadata(title, description, keywords));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Title;
        yield return Description;
        yield return Keywords;
    }
}