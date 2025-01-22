using System.Text.Json;
using System.Text.Json.Serialization;

namespace SheetGenerator.Configuration;

public class ExportConfigConverter : JsonConverter<ExportConfig>
{
    public override ExportConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var element = jsonDoc.RootElement;

        if (!element.TryGetProperty("type", out var typeElement))
        {
            throw new JsonException("Missing 'type' property in export configuration");
        }

        var type = typeElement.GetString()?.ToLower();
        return type switch
        {
                "binary" => JsonSerializer.Deserialize<BinaryExportConfig>(element.GetRawText(), options),
                "json" => JsonSerializer.Deserialize<JsonExportConfig>(element.GetRawText(), options),
                _ => throw new JsonException($"Unknown export type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ExportConfig value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
