using MessagePack;

namespace SheetGenerator.Models;

[MessagePackObject]
public class TableData
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public List<Column> Columns { get; set; } = new();

    [Key(2)]
    public List<RecordData> Records { get; set; } = new();

    [IgnoreMember]
    public int ColumnCount
    {
        get => Columns.Count;
    }

    [IgnoreMember]
    public int RecordCount
    {
        get => Records.Count;
    }
}
