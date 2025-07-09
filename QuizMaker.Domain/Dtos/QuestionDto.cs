using FluentValidation;

namespace QuizMaker.Domain.Dtos;

public class QuestionDto
{
    public long? Id { get; set; } = null;
    public string QuestionPhrase { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
}

public class QuestionDtoValidator : AbstractValidator<QuestionDto> 
{
    public QuestionDtoValidator()
    {
        When(x => x.Id == null, () =>
        {
            RuleFor(x => x.QuestionPhrase).NotNull().NotEmpty();
            RuleFor(x => x.CorrectAnswer).NotNull().NotEmpty();
        });
    }
}