using System.Composition;
using System.Reflection;
using System.Text;
using QuizMaker.Shared.Base;

namespace QuizMaker.Plugins.CsvExport;

[Export(typeof(IFileExporter))]
[ExportMetadata("Format", "text/csv")]
public class CsvExporter : IFileExporter
{
    public async Task<MemoryStream> ExportAsync<T>(IList<T> data) where T: class
    {
        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            await writer.WriteLineAsync(string.Join(",", properties.Select(p => p.Name)));
            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty);
                await writer.WriteLineAsync(string.Join(",", values));
            }

            await writer.FlushAsync();
        }

        stream.Position = 0;
        return stream;
    }
}