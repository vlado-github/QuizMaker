namespace QuizMaker.Shared.Base;

public class FileExporterFormatMetadata
{
    public string Format { get; set; }

    public FileExporterFormatMetadata() { }

    public FileExporterFormatMetadata(IDictionary<string, object> metadata)
    {
        if (metadata.TryGetValue(nameof(Format), out var format))
        {
            Format = format as string;
        }
    }
}