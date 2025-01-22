using Serilog;
using SheetGenerator.Configuration;
using SheetGenerator.IO;
using SheetGenerator.Models;

namespace SheetGenerator.Export;

public class BinaryExporter : IDataExporter
{
    private readonly BinaryExportConfig _config;

    public BinaryExporter(ExportConfig config)
    {
        _config = config as BinaryExportConfig ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task ExportAsync(List<Sheet> sheets, string outputPath)
    {
        try
        {
            if (Directory.Exists(outputPath) == false)
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (var sheet in sheets)
            {
                foreach (var table in sheet.Tables)
                {
                    var fileName = $"{table.Name}Data.{_config.Extension}";
                    var filePath = Path.Combine(outputPath, fileName);

                    Log.Information("Creating table binary files: {TableName} -> {FilePath}", table.Name, filePath);

                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    using var writer = new TableBinaryWriter(fileStream);
                    writer.WriteTable(table);
                }
            }

            Log.Information("The binary file generation of all tables has been completed.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating binary file.");
            throw;
        }
    }
}
