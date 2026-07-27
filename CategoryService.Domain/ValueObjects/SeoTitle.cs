using CategoryService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.ValueObjects;

public sealed partial class SeoTitle : ValueObject
{
    public static int MaxLength => 100;

    public string Value { get; }

    private SeoTitle(string value)
    {
        Value = value;
    }

    public static IResult<SeoTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<SeoTitle>(SeoTitleErrors.NullOrEmpty());

        value = Normalize(value);

        if (value.Length > MaxLength)
            return Fail<SeoTitle>(SeoTitleErrors.TooLong(MaxLength));

        return Ok(new SeoTitle(value));
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();

        value = NormalizeRegex().Replace(value, " ");

        return value;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeRegex();

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(SeoTitle seoTitle) => seoTitle.ToString();
}