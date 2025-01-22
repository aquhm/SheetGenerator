using System.Text.Json;
using System.Text.Json.Serialization;

namespace SheetGenerator.Configuration;

public class ImportConfigConverter : JsonConverter<ImportConfig>
{
    public override ImportConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var element = jsonDoc.RootElement;

        if (!element.TryGetProperty("type", out var typeElement))
        {
            throw new JsonException("Missing 'type' property in import configuration");
        }

        var type = typeElement.GetString()?.ToLower();
        return type switch
        {
                "googlesheet" => JsonSerializer.Deserialize<GoogleSheetImportConfig>(element.GetRawText(), options),
                "excel" => JsonSerializer.Deserialize<ExcelImportConfig>(element.GetRawText(), options),
                _ => throw new JsonException($"Unknown import type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ImportConfig value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
