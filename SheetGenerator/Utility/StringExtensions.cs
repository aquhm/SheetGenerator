namespace SheetGenerator.Util;

public static class StringExtensions
{
    public static bool IsValidTableString(this string value)
    {
        return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsValidTableCell(this object cell)
    {
        var cellValue = cell.ToString();
        return cellValue.IsValidTableString() && cellValue != "C";
    }

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if (str.Length == 1)
            return str.ToLower();

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if (str.Length == 1)
            return str.ToUpper();

        return char.ToUpperInvariant(str[0]) + str.Substring(1);
    }

    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }
}