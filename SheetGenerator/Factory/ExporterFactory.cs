using SheetGenerator.Configuration;
using SheetGenerator.Export;

namespace SheetGenerator.Factory;

public static class ExporterFactory
{
    public static IDataExporter Create(ExportConfig config)
    {
        return config.Type.ToLower() switch
        {
                "binary" => new BinaryExporter(config),
                "json" => new JsonExporter(config),
                _ => throw new ArgumentException($"지원하지 않는 익스포터 타입: {config.Type}")
        };
    }
}
