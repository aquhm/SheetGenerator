using System.Text.Json;
using Serilog;
using SheetGenerator.Configuration;
using SheetGenerator.Models;

namespace SheetGenerator.Export;

public class JsonExporter : IDataExporter
{
    private readonly JsonExportConfig _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonExporter(ExportConfig config)
    {
        _config = config as JsonExportConfig ?? throw new ArgumentNullException(nameof(config));
        _jsonOptions = new JsonSerializerOptions { WriteIndented = _config.WriteIndented, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    public async Task ExportAsync(List<Sheet> sheets, string outputPath)
    {
        try
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            foreach (var sheet in sheets)
            {
                foreach (var table in sheet.Tables)
                {
                    var tableData = new TableData { Name = table.Name, Columns = table.Columns, Records = ConvertToRecordData(table) };

                    var fileName = $"{table.Name}Data.json";
                    var filePath = Path.Combine(outputPath, fileName);

                    Log.Information("테이블 JSON 파일 생성 중: {TableName} -> {FilePath}", table.Name, filePath);

                    var json = JsonSerializer.Serialize(tableData, _jsonOptions);
                    await File.WriteAllTextAsync(filePath, json);
                }
            }

            Log.Information("모든 테이블의 JSON 파일 생성이 완료되었습니다.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "JSON 파일 생성 중 오류가 발생했습니다");
            throw;
        }
    }

    private List<RecordData> ConvertToRecordData(Table table)
    {
        var records = new List<RecordData>();

        foreach (var row in table.Rows)
        {
            var record = new RecordData();
            var cellsByIndex = row.Cells.ToDictionary(c => c.ColumnIndex);

            foreach (var column in table.Columns)
            {
                object value;

                if (cellsByIndex.TryGetValue(column.Index, out var cell))
                {
                    value = cell.Value ?? string.Empty;
                }
                else
                {
                    value = column.Type.ToLower() switch
                    {
                            "int" => 0,
                            "float" => 0.0f,
                            "double" => 0.0d,
                            "bool" => false,
                            _ => string.Empty
                    };
                }

                record.SetValue(column.Name, value);
            }

            records.Add(record);
        }

        return records;
    }
}
