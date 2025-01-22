using MessagePack;
using MessagePack.Formatters;

namespace SheetGenerator.MessagePack;

public class DynamicObjectFormatter : IMessagePackFormatter<object?>
{
    public void Serialize(ref MessagePackWriter writer, object? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }

        switch (value)
        {
            case int intValue:
                writer.Write(intValue);
                break;
            case float floatValue:
                writer.Write(floatValue);
                break;
            case double doubleValue:
                writer.Write(doubleValue);
                break;
            case bool boolValue:
                writer.Write(boolValue);
                break;
            case string strValue:
                writer.Write(strValue);
                break;
            case DateTime dateValue:
                writer.Write(dateValue.ToBinary());
                break;
            default:
                writer.Write(value.ToString());
                break;
        }
    }

    public object? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.IsNil)
        {
            reader.Skip();
            return null;
        }

        switch (reader.NextMessagePackType)
        {
            case MessagePackType.Integer:
                return reader.ReadInt32();
            case MessagePackType.Float:
                return reader.ReadDouble();
            case MessagePackType.String:
                return reader.ReadString();
            case MessagePackType.Boolean:
                return reader.ReadBoolean();
            default:
                return reader.ReadString();
        }
    }
}
