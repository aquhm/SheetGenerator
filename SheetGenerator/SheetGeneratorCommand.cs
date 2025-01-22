using System.CommandLine;
using System.CommandLine.Invocation;

namespace SheetGenerator.Commands;

public class SheetGeneratorCommand : RootCommand
{
    private readonly Option<string> _configOption;
    private readonly Option<bool> _developmentOption;
    private readonly Option<bool> _forceOption;
    private readonly Option<bool> _initOption;
    private readonly Option<string> _logLevelOption;

    public SheetGeneratorCommand() : base("SheetGenerator - 데이터 시트 처리 도구")
    {
        _configOption = new Option<string>("--config",
                "설정 파일 경로") { IsRequired = false };

        _logLevelOption = new Option<string>("--loglevel",
                () => "Information",
                "로그 레벨 설정 (Verbose, Debug, Information, Warning, Error, Fatal)") { IsRequired = false };

        // 대소문자를 구분하지 않도록 AddValidator 사용
        _logLevelOption.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<string>();
            if (value == null)
            {
                return;
            }

            var validValues = new[] { "verbose", "debug", "information", "warning", "error", "fatal" };
            if (!validValues.Contains(value.ToLowerInvariant()))
            {
                result.ErrorMessage = "로그 레벨은 다음 중 하나여야 합니다: Verbose, Debug, Information, Warning, Error, Fatal";
            }
        });

        _initOption = new Option<bool>("--init",
                "기본 환경 초기화 및 템플릿 파일 생성");

        _forceOption = new Option<bool>("--force",
                "기존 파일 강제 덮어쓰기");

        _developmentOption = new Option<bool>("--development",
                "개발 모드로 실행 (프로젝트 디렉토리를 루트로 사용)");

        AddOption(_configOption);
        AddOption(_logLevelOption);
        AddOption(_initOption);
        AddOption(_forceOption);
        AddOption(_developmentOption);

        this.SetHandler(HandleCommand);
    }

    private async Task HandleCommand(InvocationContext context)
    {
        try
        {
            var config = context.ParseResult.GetValueForOption(_configOption);
            var logLevel = context.ParseResult.GetValueForOption(_logLevelOption) ?? "Information";
            var init = context.ParseResult.GetValueForOption(_initOption);
            var force = context.ParseResult.GetValueForOption(_forceOption);
            var development = context.ParseResult.GetValueForOption(_developmentOption);

            var processor = new CommandProcessor(config, logLevel, init, force, development);
            var result = await processor.ProcessAsync();
            context.ExitCode = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"실행 중 오류 발생: {ex.Message}");
            Console.WriteLine($"스택 추적: {ex.StackTrace}");
            context.ExitCode = 1;
        }
    }
}
