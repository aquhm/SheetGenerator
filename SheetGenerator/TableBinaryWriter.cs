using MessagePack;
using SheetGenerator.Configuration;
using SheetGenerator.Models;

namespace SheetGenerator.IO;

public class TableBinaryWriter : IDisposable
{
    private readonly Stream _outputStream;

    public TableBinaryWriter(Stream outputStream)
    {
        _outputStream = outputStream;
    }

    public void Dispose()
    {
    }

    public void WriteTable(Table table)
    {
        try
        {
            var metadata = new TableMetadata
            {
                    Version = 1,
                    TableName = table.Name,
                    Description = table.Description,
                    ColumnCount = table.Columns.Count,
                    RowCount = table.Rows.Count,
                    Columns = table.Columns.OrderBy(c => c.Index).ToList()
            };

            var records = table.Rows.OrderBy(r => r.Index)
                               .Select(row => CreateRecord(row, table.Columns))
                               .ToList();

            var tableData = new TableData { Name = table.Name, Columns = metadata.Columns, Records = records };

            MessagePackSerializer.Serialize(_outputStream, tableData, MessagePackConfig.Options);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to write table {table.Name}", ex);
        }
    }

    private RecordData CreateRecord(Row row, List<Column> columns)
    {
        var record = new RecordData();

        foreach (var cell in row.Cells.OrderBy(c => c.ColumnIndex))
        {
            var column = columns.First(c => c.Index == cell.ColumnIndex);
            var value = ConvertValue(cell.Value, column.Type);

            record.SetValue(column.Name, value);
        }

        return record;
    }

    private object ConvertValue(object value, string type)
    {
        if (value is string strValue && string.IsNullOrEmpty(strValue))
        {
            // 타입별 기본값 반환
            return type.ToLower() switch
            {
                    "int" => 0,
                    "float" => 0.0f,
                    "double" => 0.0d,
                    "long" => 0L,
                    "bool" => false,
                    _ => string.Empty
            };
        }

        try
        {
            return (type.ToLower() switch
            {
                    "int" => Convert.ToInt32(value),
                    "float" => Convert.ToSingle(value),
                    "double" => Convert.ToDouble(value),
                    "bool" => Convert.ToBoolean(value),
                    "long" => Convert.ToInt64(value),
                    "string" => value.ToString(),
                    _ => value.ToString()
            })!;
        }
        catch (FormatException ex)
        {
            throw new FormatException("Cannot convert value '{value}' to type '{type}' (check column type)", ex);
        }
    }
}

[MessagePackObject]
public class TableMetadata
{
    [Key(0)]
    public uint Version { get; set; }

    [Key(1)]
    public byte CompressionType { get; set; }

    [Key(2)]
    public string TableName { get; set; }

    [Key(3)]
    public string Description { get; set; }

    [Key(4)]
    public int ColumnCount { get; set; }

    [Key(5)]
    public int RowCount { get; set; }

    [Key(6)]
    public List<Column> Columns { get; set; }
}
