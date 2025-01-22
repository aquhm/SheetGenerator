using SheetGenerator.Configuration;
using SheetGenerator.Models;

namespace SheetGenerator.Import;

public interface IDataImporter
{
    Task<List<Sheet>> ImportAsync(GeneratorSettings config);
}