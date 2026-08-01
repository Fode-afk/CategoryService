using FluentValidation;

namespace CategoryService.Application.Features.Queries.SearchCategories;

public sealed class SearchCategoriesQueryValidator : AbstractValidator<SearchCategoriesQuery>
{ 
    public SearchCategoriesQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .WithErrorCode("Category.SearchQueryRequired")
            .MinimumLength(2)
            .WithErrorCode("Category.SearchQueryTooShort")
            .MaximumLength(150)
            .WithErrorCode("Category.SearchQueryTooLong");

        RuleFor(x => x.MaxResults)
            .InclusiveBetween(1, 100)
            .WithErrorCode("Category.MaxResultsOutOfRange");
    }
}
