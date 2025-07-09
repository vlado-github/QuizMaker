using FluentValidation;
using Newtonsoft.Json;

namespace QuizMaker.Domain.Base;

public record PagedQueryParamBase(int Page = 1, int ItemsPerPage = 50)
{
    internal int Skip => (Page - 1) * ItemsPerPage;
    
    internal int Take => ItemsPerPage;
}

public class PagedQueryParamValidator : AbstractValidator<PagedQueryParamBase> 
{
    public PagedQueryParamValidator()
    {
        RuleFor(x => x.ItemsPerPage).LessThanOrEqualTo(1000);
    }
}