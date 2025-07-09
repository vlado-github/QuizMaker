using System.Composition;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Domain.Features.QuizExporter.Commands;
using QuizMaker.Shared.Base;

namespace QuizMaker.Domain.Features.QuizExporter;

public interface IExporterCommandHandler
{
    Task<MemoryStream> Handle(ExportQuizCommand command);
}

internal class ExporterCommandHandler : IExporterCommandHandler
{
    private readonly QuizMakerContext _dbContext;
    private readonly IEnumerable<Lazy<IFileExporter, FileExporterFormatMetadata>> _fileExporters;

    [ImportingConstructor]
    public ExporterCommandHandler(
        QuizMakerContext dbContext, 
        IEnumerable<Lazy<IFileExporter, FileExporterFormatMetadata>> fileExporters)
    {
        _dbContext = dbContext;
        _fileExporters = fileExporters;
    }

    public async Task<MemoryStream> Handle(ExportQuizCommand command)
    {
        var fileType = command.FileType.ToLower();
        var fileExporter = _fileExporters
            .SingleOrDefault(x => x.Metadata.Format == fileType)?.Value;
        if (fileExporter == null)
        {
            throw new NotSupportedException($"File export for file type {fileType} is not supported.");
        }

        var quizQuestions = await _dbContext.Questions
            .Where(x => x.Quizzes.Select(q => q.Id).Contains(command.Id))
            .Select(x => new { x.Id, x.QuestionPhrase })
            .ToListAsync();
        return await fileExporter.ExportAsync(quizQuestions);
    }
}

