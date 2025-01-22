using SheetGenerator.Configuration;
using SheetGenerator.Import;

namespace SheetGenerator.Factory;

public static class ImporterFactory
{
    public static IDataImporter Create(ImportConfig config)
    {
        return config.Type.ToLower() switch
        {
                "googlesheet" => new GoogleSheetImporter(config),
                _ => throw new ArgumentException($"지원하지 않는 임포터 타입: {config.Type}")
        };
    }
}