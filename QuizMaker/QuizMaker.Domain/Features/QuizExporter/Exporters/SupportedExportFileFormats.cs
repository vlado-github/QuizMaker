namespace QuizMaker.Domain.Features.QuizExporter.Exporters;

public class SupportedExportFileFormats
{
    public static readonly IDictionary<string, string> Formats = new Dictionary<string, string>
    {
        { "csv", "text/csv" }
    };
}