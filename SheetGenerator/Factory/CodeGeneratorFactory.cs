using SheetGenerator.CodeGenerate;
using SheetGenerator.Configuration;

namespace SheetGenerator.Factory;

public static class CodeGeneratorFactory
{
    public static ICodeGenerator Create(CodeGenConfig config)
    {
        return config.Type.ToLower() switch
        {
                "csharp" => new CSharpCodeGenerator(config),
                _ => throw new ArgumentException($"지원하지 않는 언어: {config.Type}")
        };
    }
}