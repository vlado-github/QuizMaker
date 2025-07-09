using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Domain.Features.QuizExporter.Base;
using QuizMaker.Domain.Features.QuizExporter.Commands;
using QuizMaker.Domain.Features.QuizExporter.Exporters;

namespace QuizMaker.Domain.Features.QuizExporter;

public interface IExporterCommandHandler
{
    Task<MemoryStream> Handle(ExportQuizCommand command);
}

internal class ExporterCommandHandler : IExporterCommandHandler
{
    [ImportMany] 
    private IEnumerable<Lazy<IFileExporter, IFileExporterFormat>> _fileExporters;
    private readonly QuizMakerContext _dbContext;

    public ExporterCommandHandler(QuizMakerContext dbContext)
    {
        _dbContext = dbContext;
        
        var catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(IFileExporter).Assembly));
        var container = new CompositionContainer(catalog);
        container.ComposeParts(this);
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

