using MessagePack;

namespace SheetGenerator.Models;

[MessagePackObject]
public class RecordData
{
    [Key(0)]
    public Dictionary<string, object> Values { get; set; } = new();

    public void SetValue(string key, object value)
    {
        Values[key] = value;
    }

    public T GetValue<T>(string columnName)
    {
        if (!Values.TryGetValue(columnName, out var value))
        {
            return default;
        }

        if (value is T typedValue)
        {
            return typedValue;
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
}
