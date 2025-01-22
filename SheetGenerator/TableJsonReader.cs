using System.Text.Json;

namespace SheetGenerator.IO;

public class TableJsonReader : IDisposable
{
    public TableJsonReader(string jsonContent)
    {
        JsonContent = jsonContent;
    }

    public string JsonContent { get; }

    public void Dispose()
    {
    }

    public T ReadJsonData<T>(JsonSerializerOptions? options = null)
    {
        try
        {
            options ??= new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<T>(JsonContent, options)
                   ?? throw new InvalidDataException("Failed to deserialize JSON data");
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Failed to read JSON data of type {typeof(T).Name}", ex);
        }
    }
}
