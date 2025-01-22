using Serilog;
using Serilog.Events;
using SheetGenerator.Configuration;
using SheetGenerator.Factory;
using SheetGenerator.Logging;
using SheetGenerator.Models;
using SheetGenerator.Util;

namespace SheetGenerator;

public class CommandProcessor
{
    private readonly CommandContext _context;

    public CommandProcessor(string configPath, string logLevel, bool initialize, bool force, bool development)
    {
        _context = new CommandContext(configPath, logLevel, initialize, force, development);
    }

    public async Task<int> ProcessAsync()
    {
        try
        {
            await InitializeEnvironment();

            if (_context.Initialize)
            {
                return await HandleInitialization();
            }

            return await ProcessConfiguration();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "처리 중 오류 발생");
            return 1;
        }
    }

    private async Task InitializeEnvironment()
    {
        PathHelper.Initialize(_context.Development);
        LoggerSetup.Configure(_context.LogLevel);
    }

    private async Task<int> HandleInitialization()
    {
        Log.Information("초기화 시작...");
        EnvironmentInitializer.Initialize(_context.Force);
        Log.Information("초기화가 성공적으로 완료되었습니다.");
        return 0;
    }

    private async Task<int> ProcessConfiguration()
    {
        var settingPath = _context.ConfigPath ?? Path.Combine(PathHelper.ProjectRootPath, "settings.json");
        if (!File.Exists(settingPath))
        {
            throw new FileNotFoundException($"설정 파일을 찾을 수 없습니다: {settingPath}");
        }

        var settings = SettingsLoader.LoadSettings(settingPath);
        await ProcessImports(settings);

        Log.Information("모든 작업이 성공적으로 완료되었습니다.");
        return 0;
    }

    private async Task ProcessImports(GeneratorSettings settings)
    {
        foreach (var importConfig in settings.Imports)
        {
            Log.Information("임포트 설정 처리 중: {Type}", importConfig.Type);
            var importer = ImporterFactory.Create(importConfig);
            var sheets = await importer.ImportAsync(settings);

            // IReadOnlyList<Sheet>를 List<Sheet>로 변환
            var sheetsList = sheets.ToList();

            await ProcessExports(settings, sheetsList);
            await ProcessCodeGeneration(settings, sheetsList);
        }
    }

    private async Task ProcessExports(GeneratorSettings settings, List<Sheet> sheets)
    {
        foreach (var exportConfig in settings.Exports)
        {
            var exportPath = Path.Combine(PathHelper.ProjectRootPath, exportConfig.OutputPath);
            Log.Information("내보내기 설정 처리 중: {Type} -> {Path}", exportConfig.Type, exportPath);
            var exporter = ExporterFactory.Create(exportConfig);
            await exporter.ExportAsync(sheets, exportPath);
        }
    }

    private async Task ProcessCodeGeneration(GeneratorSettings settings, List<Sheet> sheets)
    {
        foreach (var codeGenConfig in settings.CodeGens)
        {
            var codeGenPath = Path.Combine(PathHelper.ProjectRootPath, codeGenConfig.OutputPath);
            Log.Information("코드 생성 처리 중: {Type} -> {Path}", codeGenConfig.Type, codeGenPath);
            var generator = CodeGeneratorFactory.Create(codeGenConfig);
            await generator.GenerateAsync(sheets, codeGenPath);
        }
    }
}

public class CommandContext
{
    public CommandContext(string? configPath, string logLevel, bool initialize, bool force, bool development)
    {
        ConfigPath = configPath;
        LogLevelString = logLevel;
        Initialize = initialize;
        Force = force;
        Development = development;
    }

    public string? ConfigPath { get; }
    public string LogLevelString { get; }

    public LogEventLevel LogLevel
    {
        get => ParseLogLevel(LogLevelString);
    }

    public bool Initialize { get; }
    public bool Force { get; }
    public bool Development { get; }

    private LogEventLevel ParseLogLevel(string level)
    {
        if (Enum.TryParse<LogEventLevel>(level, true, out var result))
        {
            return result;
        }

        return LogEventLevel.Information;
    }
}
