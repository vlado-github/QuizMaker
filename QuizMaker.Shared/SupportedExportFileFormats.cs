namespace QuizMaker.Shared;

public class SupportedExportFileFormats
{
    public static readonly IDictionary<string, string> Formats = new Dictionary<string, string>
    {
        { "text/csv", "csv" }
    };
}