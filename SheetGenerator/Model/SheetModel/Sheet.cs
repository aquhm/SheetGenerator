using System.Runtime.CompilerServices;
using MessagePack;

namespace SheetGenerator.Models;

[MessagePackObject]
public class Sheet
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public List<Table> Tables { get; set; } = new();
}

[MessagePackObject]
public class Table
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Description { get; set; }

    [Key(2)]
    public int StartRowIndex { get; set; }

    [Key(3)]
    public int StartColumnIndex { get; set; }

    [Key(4)]
    public List<Column> Columns { get; set; } = new();

    [Key(5)]
    public List<Row> Rows { get; set; } = new();
}

[MessagePackObject]
public class Column
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Type { get; set; }

    [Key(2)]
    public string Description { get; set; }

    [Key(3)]
    public int Index { get; set; }
}

[MessagePackObject]
public class Row
{
    [Key(0)]
    public int Index { get; set; }

    [Key(1)]
    public List<Cell> Cells { get; set; } = new();
}

[MessagePackObject]
public class Cell
{
    private static readonly HashSet<Type> NumericTypes = new()
    {
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(short),
            typeof(byte),
            typeof(sbyte),
            typeof(uint),
            typeof(ulong),
            typeof(ushort)
    };

    [SerializationConstructor]
    public Cell() { }

    public Cell(object value, int columnIndex)
    {
        Value = value;
        ColumnIndex = columnIndex;
    }

    [Key(0)]
    public object Value { get; set; }

    [Key(1)]
    public int ColumnIndex { get; set; }

    public T GetValue<T>()
    {
        try
        {
            if (Value == null)
            {
                return default;
            }

            if (Value is T typedValue)
            {
                return typedValue;
            }

            if (typeof(T) == typeof(DateTime) && Value is string strValue)
            {
                return (T)(object)DateTime.Parse(strValue);
            }

            if (IsNumericType(typeof(T)) && IsNumericType(Value.GetType()))
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }

            if (typeof(T) == typeof(bool))
            {
                if (Value is string boolStr)
                {
                    if (bool.TryParse(boolStr, out var result))
                    {
                        return (T)(object)result;
                    }

                    return (T)(object)(boolStr != "0");
                }

                if (Value is int intValue)
                {
                    return (T)(object)(intValue != 0);
                }
            }

            return (T)Convert.ChangeType(Value, typeof(T));
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert cell value '{Value}' ({Value?.GetType().Name ?? "null"}) to type {typeof(T).Name}", ex);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNumericType(Type type)
    {
        return NumericTypes.Contains(type);
    }

    public bool TryGetValue<T>(out T result)
    {
        try
        {
            result = GetValue<T>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}
