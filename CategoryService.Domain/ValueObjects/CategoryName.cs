using CategoryService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.ValueObjects;

public sealed partial class CategoryName : ValueObject
{
    public static int MinLength => 2;
    public static int MaxLength => 150;

    public string Value { get; }

    private CategoryName(string value)
    {
        Value = value;
    }

    public static IResult<CategoryName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<CategoryName>(CategoryNameErrors.NullOrEmpty());

        value = Normalize(value);

        if (value.Length < MinLength)
            return Fail<CategoryName>(CategoryNameErrors.TooShort(MinLength));

        if (value.Length > MaxLength)
            return Fail<CategoryName>(CategoryNameErrors.TooLong(MaxLength));

        return Ok(new CategoryName(value));
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

    public static implicit operator string(CategoryName categoryName) => categoryName.ToString();
}