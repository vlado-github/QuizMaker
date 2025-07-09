namespace QuizMaker.Shared.Base;

public interface IFileExporter
{
    Task<MemoryStream> ExportAsync<T>(IList<T> reportObjects) where T: class;
}