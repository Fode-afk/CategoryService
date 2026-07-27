using CategoryService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.ValueObjects;

public sealed class SeoKeywords : ValueObject
{
    public static int MaxLength => 500;

    public string Value { get; }

    private SeoKeywords(string value)
    {
        Value = value;
    }

    public static IResult<SeoKeywords> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<SeoKeywords>(SeoKeywordsErrors.NullOrEmpty());

        value = NormalizeKeywords(value);

        if (value.Length > MaxLength)
            return Fail<SeoKeywords>(SeoKeywordsErrors.TooLong(MaxLength));

        return Ok(new SeoKeywords(value));
    }

    private static string NormalizeKeywords(string value)
    {
        var keywords = value
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.ToLowerInvariant())
            .Distinct();

        return string.Join(", ", keywords);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(SeoKeywords seoKeywords) => seoKeywords.ToString();
}
