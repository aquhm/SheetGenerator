public static class ValueConverter
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

    public static object ConvertValue(object value, Type targetType)
    {
        try
        {
            if (value == null)
            {
                return GetDefaultValue(targetType);
            }

            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            if (targetType == typeof(DateTime) && value is string strValue)
            {
                return DateTime.Parse(strValue);
            }

            if (IsNumericType(targetType) && IsNumericType(value.GetType()))
            {
                return Convert.ChangeType(value, targetType);
            }

            if (targetType == typeof(bool))
            {
                if (value is string boolStr)
                {
                    if (bool.TryParse(boolStr, out var result))
                    {
                        return result;
                    }

                    return boolStr != "0";
                }

                if (value is int intValue)
                {
                    return intValue != 0;
                }
            }

            if (targetType == typeof(string))
            {
                return value.ToString() ?? string.Empty;
            }

            if (value is IConvertible)
            {
                return Convert.ChangeType(value, targetType);
            }

            return GetDefaultValue(targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"값 '{value}' ({value?.GetType().Name ?? "null"})을 {targetType.Name} 타입으로 변환할 수 없습니다.",
                    ex);
        }
    }

    private static bool IsNumericType(Type type)
    {
        return NumericTypes.Contains(type);
    }

    private static object GetDefaultValue(Type type)
    {
        if (type == typeof(string))
        {
            return string.Empty;
        }

        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
