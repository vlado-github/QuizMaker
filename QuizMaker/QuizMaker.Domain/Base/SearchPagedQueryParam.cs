using FluentValidation;

namespace QuizMaker.Domain.Base;

public record SearchPagedQueryParam(string? Search = null) : PagedQueryParamBase;

public class SearchPagedQueryParamValidator : AbstractValidator<SearchPagedQueryParam> 
{
    public SearchPagedQueryParamValidator()
    {
        RuleFor(x => x.ItemsPerPage).LessThanOrEqualTo(1000);
    }
}