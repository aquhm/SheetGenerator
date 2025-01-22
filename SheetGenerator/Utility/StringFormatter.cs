using System.Buffers;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace StaticData;

public static class StringFormatter
{
    private static readonly string[] SingleElementCache = { "[]", "{}", "\"\"" };

    /// <summary>
    ///     Appends the string representation of an object to the StringBuilder.
    /// </summary>
    public static void Format(object? value, StringBuilder builder, bool isFirst = true)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (value == null)
        {
            builder.Append("null");
            return;
        }

        if (!isFirst)
        {
            builder.Append(", ");
        }

        switch (value)
        {
            case string str:
                FormatString(str, builder);
                break;
            case IDictionary dict:
                FormatDictionary(dict, builder);
                break;
            case IEnumerable enumerable:
                FormatEnumerable(enumerable, builder);
                break;
            case double d:
                builder.Append(d.ToString("G", CultureInfo.InvariantCulture));
                break;
            case float f:
                builder.Append(f.ToString("G", CultureInfo.InvariantCulture));
                break;
            case decimal m:
                builder.Append(m.ToString(CultureInfo.InvariantCulture));
                break;
            case DateTime dt:
                builder.Append('\"').Append(dt.ToString("O")).Append('\"');
                break;
            default:
                builder.Append(value);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FormatString(string value, StringBuilder builder)
    {
        if (string.IsNullOrEmpty(value))
        {
            builder.Append(SingleElementCache[2]);
            return;
        }

        builder.Append('\"');
        var startIndex = builder.Length;
        builder.Append(value);

        for (var i = startIndex; i < builder.Length; i++)
        {
            var c = builder[i];
            if (NeedsEscaping(c))
            {
                builder.Insert(i, '\\');
                i++;
            }
        }

        builder.Append('\"');
    }

    private static void FormatDictionary(IDictionary dict, StringBuilder builder)
    {
        if (dict.Count == 0)
        {
            builder.Append(SingleElementCache[1]);
            return;
        }

        builder.Append('{');
        var isFirst = true;

        foreach (DictionaryEntry entry in dict)
        {
            Format(entry.Key, builder, isFirst);
            builder.Append(": ");
            Format(entry.Value, builder);
            isFirst = false;
        }

        builder.Append('}');
    }

    private static void FormatEnumerable(IEnumerable enumerable, StringBuilder builder)
    {
        if (enumerable is Array { Length: 0 })
        {
            builder.Append(SingleElementCache[0]);
            return;
        }

        builder.Append('[');
        var isFirst = true;

        if (enumerable is ICollection collection)
        {
            var count = collection.Count;
            if (count > 0)
            {
                object?[] buffer = ArrayPool<object?>.Shared.Rent(count);
                try
                {
                    CopyToBuffer(enumerable, buffer.AsSpan(0, count));
                    FormatBuffer(buffer.AsSpan(0, count), builder, ref isFirst);
                }
                finally
                {
                    ArrayPool<object?>.Shared.Return(buffer);
                }
            }
        }
        else
        {
            foreach (var item in enumerable)
            {
                Format(item, builder, isFirst);
                isFirst = false;
            }
        }

        builder.Append(']');
    }

    private static void CopyToBuffer(IEnumerable source, Span<object?> buffer)
    {
        var index = 0;
        foreach (var item in source)
        {
            buffer[index++] = item;
        }
    }

    private static void FormatBuffer(ReadOnlySpan<object?> buffer, StringBuilder builder, ref bool isFirst)
    {
        foreach (var item in buffer)
        {
            Format(item, builder, isFirst);
            isFirst = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool NeedsEscaping(char c)
    {
        return c switch
        {
                '"' or '\\' or '\b' or '\f' or '\n' or '\r' or '\t' => true,
                _ => c < ' '
        };
    }
}
