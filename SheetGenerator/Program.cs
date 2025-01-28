//--loglevel debug --config C:/Programming/Projects/c#/SheetGenerator/bin/settings.json

using System.CommandLine;
using Serilog.Events;
using SheetGenerator.Commands;
using SheetGenerator.Logging;

namespace SheetGenerator;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            LoggerSetup.Configure(LogEventLevel.Information);

            var rootCommand = new SheetGeneratorCommand();
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"프로그램 실행 중 오류 발생: {ex.Message}");
            Console.WriteLine($"스택 추적: {ex.StackTrace}");
            return 1;
        }
    }
}
