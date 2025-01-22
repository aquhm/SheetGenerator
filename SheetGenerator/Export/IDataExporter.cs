using SheetGenerator.Models;

namespace SheetGenerator.Export;

public interface IDataExporter
{
    Task ExportAsync(List<Sheet> sheets, string outputPath);
}