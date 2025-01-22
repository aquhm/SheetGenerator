using System.Text.Json;
using System.Text.Json.Serialization;

namespace SheetGenerator.Configuration;

public class CodeGenConfigConverter : JsonConverter<CodeGenConfig>
{
    public override CodeGenConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var element = jsonDoc.RootElement;

        if (!element.TryGetProperty("type", out var typeElement))
        {
            throw new JsonException("Missing 'type' property in code generation configuration");
        }

        var type = typeElement.GetString()?.ToLower();
        return type switch
        {
                "csharp" => JsonSerializer.Deserialize<CSharpCodeGenConfig>(element.GetRawText(), options),
                "cpp" => JsonSerializer.Deserialize<CppCodeGenConfig>(element.GetRawText(), options),
                _ => throw new JsonException($"Unknown code generation type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, CodeGenConfig value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
