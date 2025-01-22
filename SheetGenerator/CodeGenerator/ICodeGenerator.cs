using SheetGenerator.Models;

namespace SheetGenerator.CodeGenerate;

public interface ICodeGenerator
{
    Task GenerateAsync(List<Sheet> sheets, string outputPath);
}