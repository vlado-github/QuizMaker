using FluentValidation;
using QuizMaker.Domain.Dtos;

namespace QuizMaker.Domain.Features.QuizExporter.Commands;

public class ExportQuizCommand
{
    public long Id { get; set; }
    public string FileType { get; set; }
}

public class ExportQuizCommandValidator : AbstractValidator<ExportQuizCommand>
{
    public ExportQuizCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.FileType).NotNull().NotEmpty();
    }
}