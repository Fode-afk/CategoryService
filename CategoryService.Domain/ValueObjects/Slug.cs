using CategoryService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using System.Text.RegularExpressions;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.ValueObjects;

public sealed partial class Slug : ValueObject
{
    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled)]
    private static partial Regex SlugRegex();
    public static int MaxLength => 100;
    public static int MinLength => 3;


    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static IResult<Slug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<Slug>(SlugErrors.NullOrEmpty());

        value = value.Trim().ToLowerInvariant();

        if (value.Length > MaxLength)
            return Fail<Slug>(SlugErrors.TooLong(MaxLength));

        if (value.Length < MinLength)
            return Fail<Slug>(SlugErrors.TooShort(MinLength));

        if (!SlugRegex().IsMatch(value))
            return Fail<Slug>(SlugErrors.InvalidFormat());

        return Ok(new Slug(value));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.ToString();
}
