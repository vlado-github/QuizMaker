using System.ComponentModel.Composition;

namespace QuizMaker.Domain.Features.QuizExporter.Base;

[InheritedExport]
public interface IFileExporter
{
    Task<MemoryStream> ExportAsync<T>(IList<T> reportObjects) where T: class;
}