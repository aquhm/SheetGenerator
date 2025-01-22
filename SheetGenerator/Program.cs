// using CommandLine;
// using Serilog;
// using SheetGenerator;
// using SheetGenerator.Configuration;
// using SheetGenerator.Factory;
// using SheetGenerator.Logging;
// using SheetGenerator.Util;
//
// //--init --loglevel debug --config C:\Programming\Projects\c#\SheetGenerator\SheetGenerator\settings.json
// public class Program
// {
//     public static async Task<int> Main(string[] args)
//     {
//         try
//         {
//             var result = Parser.Default.ParseArguments<Options>(args);
//
//             if (result.Errors.Any())
//             {
//                 foreach (var error in result.Errors)
//                 {
//                     Console.WriteLine($"Error: {error}");
//                 }
//
//                 return 1;
//             }
//
//             var options = result.Value;
//             await RunAsync(options);
//             return 0;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Fatal error: {ex.Message}");
//             return 1;
//         }
//     }
//
//     private static async Task RunAsync(Options options)
//     {
//         try
//         {
//             PathHelper.Initialize(options.DevelopmentMode);
//             LoggerSetup.Configure(options.LogEventLevel);
//
//             if (options.Initialize)
//             {
//                 EnvironmentInitializer.Initialize(options.Force);
//                 return;
//             }
//
//             var settingPath = options.ConfigPath ?? Path.Combine(PathHelper.ProjectRootPath, "settings.json");
//             if (!File.Exists(settingPath))
//             {
//                 throw new FileNotFoundException($"Configuration file not found: {settingPath}");
//             }
//
//             Log.Information("Bringing up configuration file: {ConfigPath}", settingPath);
//             var settings = SettingsLoader.LoadSettings(settingPath);
//
//             foreach (var importConfig in settings.Imports)
//             {
//                 Log.Information("Processing import settings: {Type}", importConfig.Type);
//                 var importer = ImporterFactory.Create(importConfig);
//                 var sheets = await importer.ImportAsync(settings);
//                 Log.Information("Sheet import completed: {SheetCount}", sheets.Count);
//
//                 foreach (var exportConfig in settings.Exports)
//                 {
//                     var exportPath = Path.Combine(PathHelper.ProjectRootPath, exportConfig.OutputPath);
//                     Log.Information("Processing export settings: {Type} -> {Path}", exportConfig.Type, exportPath);
//                     var exporter = ExporterFactory.Create(exportConfig);
//                     await exporter.ExportAsync(sheets, exportPath);
//                 }
//
//                 foreach (var codeGenConfig in settings.CodeGens)
//                 {
//                     var codeGenPath = Path.Combine(PathHelper.ProjectRootPath, codeGenConfig.OutputPath);
//                     Log.Information("Processing code generation: {Type} -> {Path}", codeGenConfig.Type, codeGenPath);
//                     var generator = CodeGeneratorFactory.Create(codeGenConfig);
//                     await generator.GenerateAsync(sheets, codeGenPath);
//                 }
//             }
//
//             Log.Information("All processes have been successfully completed.");
//         }
//         catch (Exception ex)
//         {
//             Log.Error(ex, "An error occurred during execution");
//             throw;
//         }
//         //
//         //
//         // // 테이블 파일이 있는 경로 설정
//         // var tablePath = Path.Combine(projectRoot, settings.Exports.First().OutputPath);
//         // Log.Information("Table Path: {Path}", tablePath);
//         //
//         // // TableSystem 초기화 및 데이터 로드
//         // await TableSystem.Instance.InitializeAsync(tablePath);
//         //
//         // // 로컬라이제이션 테이블 데이터 검증
//         // ValidateLocalizationTable();
//         //
//         // Log.Information("Table system test completed successfully.");
//     }
//
//     // private static void ValidateLocalizationTable()
//     // {
//     //     var table = TableSystem.ClientTest;
//     //     var table1 = TableSystem.ClientTest2;
//     //     var table2 = TableSystem.ClientTest3;
//     //     var table3 = TableSystem.ClientTest4;
//     //
//     //     if (table == null)
//     //     {
//     //         throw new InvalidOperationException("The localization table has not been loaded.");
//     //     }
//     //
//     //     Log.Information("Localization table record count: {Count}", table.Records.Count);
//     //
//     //     // 첫 번째 레코드 출력
//     //     if (table.Records.Count > 0)
//     //     {
//     //         var firstRecord = table.Records[0];
//     //         Log.Information("첫 번째 레코드 정보:");
//     //         Log.Information("Index: {Index}", firstRecord.Index);
//     //         Log.Information("Key: {Key}", firstRecord.Key);
//     //         Log.Information("Korean: {Korean}", firstRecord.Korean);
//     //         Log.Information("English: {English}", firstRecord.English);
//     //     }
//     //
//     //     // 인덱스로 레코드 검색 테스트
//     //     try
//     //     {
//     //         var record = table.GetByIndex(1);
//     //         Log.Information("인덱스 1번 레코드 찾기 성공: {Key}", record.Key);
//     //     }
//     //     catch (KeyNotFoundException)
//     //     {
//     //         Log.Warning("인덱스 1번 레코드를 찾을 수 없습니다.");
//     //     }
//     //
//     //     // 키로 레코드 검색 테스트
//     //     if (table.Records.Count > 0)
//     //     {
//     //         var firstKey = table.Records[0].Key;
//     //         try
//     //         {
//     //             var record = table.GetByKey(firstKey);
//     //             Log.Information("키 '{Key}'로 레코드 찾기 성공", firstKey);
//     //         }
//     //         catch (KeyNotFoundException)
//     //         {
//     //             Log.Warning("키 '{Key}'로 레코드를 찾을 수 없습니다.", firstKey);
//     //         }
//     //     }
//     // }
// }


// using CommandLine;
// using Serilog;
// using SheetGenerator;
// using SheetGenerator.Configuration;
// using SheetGenerator.Factory;
// using SheetGenerator.Logging;
// using SheetGenerator.Util;
//
// //--init --loglevel debug --config C:\Programming\Projects\c#\SheetGenerator\SheetGenerator\settings.json
// public class Program
// {
//     public static async Task<int> Main(string[] args)
//     {
//         try
//         {
//             var result = Parser.Default.ParseArguments<Options>(args);
//
//             if (result.Errors.Any())
//             {
//                 foreach (var error in result.Errors)
//                 {
//                     Console.WriteLine($"Error: {error}");
//                 }
//
//                 return 1;
//             }
//
//             var options = result.Value;
//             await RunAsync(options);
//             return 0;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Fatal error: {ex.Message}");
//             return 1;
//         }
//     }
//
//     private static async Task RunAsync(Options options)
//     {
//         try
//         {
//             PathHelper.Initialize(options.DevelopmentMode);
//             LoggerSetup.Configure(options.LogEventLevel);
//
//             if (options.Initialize)
//             {
//                 EnvironmentInitializer.Initialize(options.Force);
//                 return;
//             }
//
//             var settingPath = options.ConfigPath ?? Path.Combine(PathHelper.ProjectRootPath, "settings.json");
//             if (!File.Exists(settingPath))
//             {
//                 throw new FileNotFoundException($"Configuration file not found: {settingPath}");
//             }
//
//             Log.Information("Bringing up configuration file: {ConfigPath}", settingPath);
//             var settings = SettingsLoader.LoadSettings(settingPath);
//
//             foreach (var importConfig in settings.Imports)
//             {
//                 Log.Information("Processing import settings: {Type}", importConfig.Type);
//                 var importer = ImporterFactory.Create(importConfig);
//                 var sheets = await importer.ImportAsync(settings);
//                 Log.Information("Sheet import completed: {SheetCount}", sheets.Count);
//
//                 foreach (var exportConfig in settings.Exports)
//                 {
//                     var exportPath = Path.Combine(PathHelper.ProjectRootPath, exportConfig.OutputPath);
//                     Log.Information("Processing export settings: {Type} -> {Path}", exportConfig.Type, exportPath);
//                     var exporter = ExporterFactory.Create(exportConfig);
//                     await exporter.ExportAsync(sheets, exportPath);
//                 }
//
//                 foreach (var codeGenConfig in settings.CodeGens)
//                 {
//                     var codeGenPath = Path.Combine(PathHelper.ProjectRootPath, codeGenConfig.OutputPath);
//                     Log.Information("Processing code generation: {Type} -> {Path}", codeGenConfig.Type, codeGenPath);
//                     var generator = CodeGeneratorFactory.Create(codeGenConfig);
//                     await generator.GenerateAsync(sheets, codeGenPath);
//                 }
//             }
//
//             Log.Information("All processes have been successfully completed.");
//         }
//         catch (Exception ex)
//         {
//             Log.Error(ex, "An error occurred during execution");
//             throw;
//         }
//         //
//         //
//         // // 테이블 파일이 있는 경로 설정
//         // var tablePath = Path.Combine(projectRoot, settings.Exports.First().OutputPath);
//         // Log.Information("Table Path: {Path}", tablePath);
//         //
//         // // TableSystem 초기화 및 데이터 로드
//         // await TableSystem.Instance.InitializeAsync(tablePath);
//         //
//         // // 로컬라이제이션 테이블 데이터 검증
//         // ValidateLocalizationTable();
//         //
//         // Log.Information("Table system test completed successfully.");
//     }
//
//     // private static void ValidateLocalizationTable()
//     // {
//     //     var table = TableSystem.ClientTest;
//     //     var table1 = TableSystem.ClientTest2;
//     //     var table2 = TableSystem.ClientTest3;
//     //     var table3 = TableSystem.ClientTest4;
//     //
//     //     if (table == null)
//     //     {
//     //         throw new InvalidOperationException("The localization table has not been loaded.");
//     //     }
//     //
//     //     Log.Information("Localization table record count: {Count}", table.Records.Count);
//     //
//     //     // 첫 번째 레코드 출력
//     //     if (table.Records.Count > 0)
//     //     {
//     //         var firstRecord = table.Records[0];
//     //         Log.Information("첫 번째 레코드 정보:");
//     //         Log.Information("Index: {Index}", firstRecord.Index);
//     //         Log.Information("Key: {Key}", firstRecord.Key);
//     //         Log.Information("Korean: {Korean}", firstRecord.Korean);
//     //         Log.Information("English: {English}", firstRecord.English);
//     //     }
//     //
//     //     // 인덱스로 레코드 검색 테스트
//     //     try
//     //     {
//     //         var record = table.GetByIndex(1);
//     //         Log.Information("인덱스 1번 레코드 찾기 성공: {Key}", record.Key);
//     //     }
//     //     catch (KeyNotFoundException)
//     //     {
//     //         Log.Warning("인덱스 1번 레코드를 찾을 수 없습니다.");
//     //     }
//     //
//     //     // 키로 레코드 검색 테스트
//     //     if (table.Records.Count > 0)
//     //     {
//     //         var firstKey = table.Records[0].Key;
//     //         try
//     //         {
//     //             var record = table.GetByKey(firstKey);
//     //             Log.Information("키 '{Key}'로 레코드 찾기 성공", firstKey);
//     //         }
//     //         catch (KeyNotFoundException)
//     //         {
//     //             Log.Warning("키 '{Key}'로 레코드를 찾을 수 없습니다.", firstKey);
//     //         }
//     //     }
//     // }
// }


// using CommandLine;
// using Serilog;
// using Serilog.Events;
// using SheetGenerator.Configuration;
// using SheetGenerator.Logging;
// using SheetGenerator.Util;
//
// namespace SheetGenerator;
//
// public class Program
// {
//     public static async Task<int> Main(string[] args)
//     {
//         LoggerSetup.Configure(LogEventLevel.Information);
//         try
//         {
//             var parser = new Parser(with =>
//             {
//                 with.CaseInsensitiveEnumValues = true;
//                 with.HelpWriter = Console.Out;
//                 with.AutoHelp = true;
//                 with.AutoVersion = true;
//                 with.IgnoreUnknownArguments = false;
//             });
//
//             return await parser.ParseArguments<Options>(args)
//                                .MapResult(async options => await ExecuteWithOptions(options),
//                                        errors => Task.FromResult(HandleParseErrors(errors)));
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"실행 중 오류 발생: {ex.Message}");
//             return 1;
//         }
//     }
//
//     private static async Task<int> ExecuteWithOptions(Options options)
//     {
//         try
//         {
//             Console.WriteLine("경로 초기화 중...");
//             PathHelper.Initialize(options.DevelopmentMode);
//
//             Console.WriteLine("로거 설정 중...");
//             LoggerSetup.Configure(options.LogEventLevel);
//
//             if (options.Initialize)
//             {
//                 Console.WriteLine("초기화 시작...");
//                 EnvironmentInitializer.Initialize(options.Force);
//                 Console.WriteLine("초기화가 성공적으로 완료되었습니다.");
//                 return 0;
//             }
//
//             var settingPath = options.ConfigPath ?? Path.Combine(PathHelper.ProjectRootPath, "settings.json");
//             // ... 나머지 구현 ...
//             return 0;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"실행 중 오류 발생: {ex.Message}");
//             Console.WriteLine($"스택 추적: {ex.StackTrace}");
//             return 1;
//         }
//     }
//
//     private static int HandleParseErrors(IEnumerable<Error> errors)
//     {
//         foreach (var error in errors)
//         {
//             Console.WriteLine($"명령행 파싱 오류: {error}");
//         }
//
//         return 1;
//     }
//
//     private static async Task<int> HandleParsedOptions(Options options)
//     {
//         try
//         {
//             Console.WriteLine("Initializing path helper...");
//             PathHelper.Initialize(options.DevelopmentMode);
//
//             Console.WriteLine("Setting up logger...");
//             LoggerSetup.Configure(options.LogEventLevel);
//
//             if (options.Initialize)
//             {
//                 Console.WriteLine("Starting initialization...");
//                 EnvironmentInitializer.Initialize(options.Force);
//                 Console.WriteLine("Initialization completed successfully.");
//                 return 0;
//             }
//
//             // 여기서부터는 --init이 아닌 경우의 로직
//             var settingPath = options.ConfigPath ?? Path.Combine(PathHelper.ProjectRootPath, "settings.json");
//             if (!File.Exists(settingPath))
//             {
//                 throw new FileNotFoundException($"Configuration file not found: {settingPath}");
//             }
//
//             Log.Information("Processing configuration file: {ConfigPath}", settingPath);
//             var settings = SettingsLoader.LoadSettings(settingPath);
//
//             foreach (var importConfig in settings.Imports)
//             {
//                 // ... 이하 기존 코드 ...
//             }
//
//             Log.Information("All processes completed successfully.");
//             return 0;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error: {ex.Message}");
//             Console.WriteLine($"Stack trace: {ex.StackTrace}");
//             return 1;
//         }
//     }
//
//     private static int HandleNotParsedErrors(IEnumerable<Error> errors)
//     {
//         Console.WriteLine("명령줄 인수 파싱 중 오류가 발생했습니다:");
//         foreach (var error in errors)
//         {
//             Console.WriteLine($"- {error}");
//         }
//
//         return 1;
//     }
//
//     public static async Task RunAsync(Options options)
//     {
//         try
//         {
//             Console.WriteLine("Initializing path helper...");
//             PathHelper.Initialize(options.DevelopmentMode);
//
//             Console.WriteLine("Setting up logger...");
//             LoggerSetup.Configure(options.LogEventLevel);
//
//             if (options.Initialize)
//             {
//                 Console.WriteLine("Starting initialization...");
//                 EnvironmentInitializer.Initialize(options.Force);
//                 Console.WriteLine("Initialization completed successfully.");
//                 return;
//             }
//
//             // 여기서부터는 --init이 아닌 경우의 로직
//             var settingPath = options.ConfigPath ?? Path.Combine(PathHelper.ProjectRootPath, "settings.json");
//             if (!File.Exists(settingPath))
//             {
//                 throw new FileNotFoundException($"Configuration file not found: {settingPath}");
//             }
//
//             Log.Information("Processing configuration file: {ConfigPath}", settingPath);
//             var settings = SettingsLoader.LoadSettings(settingPath);
//
//             foreach (var importConfig in settings.Imports)
//             {
//                 // ... 이하 기존 코드 ...
//             }
//
//             Log.Information("All processes completed successfully.");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error in RunAsync implementation: {ex.Message}");
//             Console.WriteLine($"Stack trace: {ex.StackTrace}");
//             throw;
//         }
//     }
// }

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
            // 초기 로거 설정
            LoggerSetup.Configure(LogEventLevel.Information);

            // SheetGeneratorCommand 인스턴스 생성 및 실행
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
