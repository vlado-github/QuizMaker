namespace QuizMaker.Domain.Features.QuizExporter.Base;

public interface IFileExporterFormat
{
    string Format { get; }
}