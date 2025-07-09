using FluentValidation;
using QuizMaker.Domain.Dtos;

namespace QuizMaker.Domain.Features.QuizBuilder.Commands;

public record CreateQuizCommand(string Name, IReadOnlyCollection<QuestionDto> Questions);

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Questions).NotNull().NotEmpty();
        RuleForEach(x => x.Questions).SetValidator(new QuestionDtoValidator());
    }
}
