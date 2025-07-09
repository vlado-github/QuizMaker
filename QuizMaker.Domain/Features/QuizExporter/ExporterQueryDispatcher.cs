using System.Composition;
using QuizMaker.Shared.Base;

namespace QuizMaker.Domain.Features.QuizExporter;

public interface IExporterQueryDispatcher
{
    IList<string> GetSupportedExportFormats();
}

public class ExporterQueryDispatcher : IExporterQueryDispatcher
{
    private readonly IEnumerable<Lazy<IFileExporter, FileExporterFormatMetadata>> _fileExporters;

    [ImportingConstructor]
    public ExporterQueryDispatcher(
        IEnumerable<Lazy<IFileExporter, FileExporterFormatMetadata>> fileExporters)
    {
        _fileExporters = fileExporters;
    }
    
    public IList<string> GetSupportedExportFormats()
    {
        return _fileExporters
            .Select(x => x.Metadata.Format)
            .ToList();
    }
}