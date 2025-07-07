using System.Collections.ObjectModel;
using FluentValidation;
using QuizMaker.Domain.Dtos;

namespace QuizMaker.Domain.QuizFeature.Commands;

public record UpdateQuizCommand(
    long Id,
    string Name,
    IReadOnlyCollection<QuestionDto>? Questions = null);

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}