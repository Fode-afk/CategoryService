using CategoryService.Domain.Errors;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Results;
using static migApp.Shared.Results.ResultFactory;

namespace CategoryService.Domain.ValueObjects;

public sealed class ImageUrl : ValueObject
{
    public string Value { get; }

    private ImageUrl(string value)
    {
        Value = value;
    }

    public static IResult<ImageUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Fail<ImageUrl>(ImageUrlErrors.NullOrEmpty());

        value = value.Trim();

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uriResult)
            || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            return Fail<ImageUrl>(ImageUrlErrors.InvalidFormat());      

        return Ok(new ImageUrl(uriResult.ToString()));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ImageUrl imageUrl) => imageUrl.ToString();
}
