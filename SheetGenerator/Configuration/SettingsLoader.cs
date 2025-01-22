using System.Text.Json;
using SheetGenerator.Util;

namespace SheetGenerator.Configuration;

public class SettingsLoader
{
    public static GeneratorSettings LoadSettings(string configPath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true, Converters = { new ImportConfigConverter(), new ExportConfigConverter(), new CodeGenConfigConverter() } };

        var json = File.ReadAllText(configPath);
        var settings = JsonSerializer.Deserialize<GeneratorSettings>(json, options)
                       ?? throw new JsonException("Failed to deserialize settings");

        Validate(settings);
        return settings;
    }

    private static void Validate(GeneratorSettings settings)
    {
        foreach (var import in settings.Imports)
        {
            switch (import)
            {
                case GoogleSheetImportConfig googleConfig:
                    if (googleConfig.SpreadsheetId.IsNullOrEmpty())
                    {
                        throw new ArgumentException("GoogleSheet import requires SpreadsheetId");
                    }

                    if (googleConfig.Auth.ApiKey.IsNullOrEmpty() &&
                        googleConfig.Auth.ServiceAccountPath.IsNullOrEmpty())
                    {
                        throw new ArgumentException("GoogleSheet import requires either ApiKey or ServiceAccountPath");
                    }

                    break;

                case ExcelImportConfig excelConfig:
                    if (excelConfig.FilePath.IsNullOrEmpty())
                    {
                        throw new ArgumentException("Excel import requires FilePath");
                    }

                    if (!File.Exists(excelConfig.FilePath))
                    {
                        throw new FileNotFoundException($"Excel file not found: {excelConfig.FilePath}");
                    }

                    break;
            }
        }

        foreach (var export in settings.Exports)
        {
            if (export.OutputPath.IsNullOrEmpty())
            {
                throw new ArgumentException($"Export configuration of type {export.Type} requires OutputPath");
            }

            if (export is BinaryExportConfig binaryConfig)
            {
                if (binaryConfig.Extension.IsNullOrEmpty())
                {
                    throw new ArgumentException("Binary export requires Extension");
                }
            }
        }

        foreach (var codeGen in settings.CodeGens)
        {
            if (codeGen.OutputPath.IsNullOrEmpty())
            {
                throw new ArgumentException($"CodeGen configuration of type {codeGen.Type} requires OutputPath");
            }

            if (codeGen.Namespace.IsNullOrEmpty())
            {
                throw new ArgumentException($"CodeGen configuration of type {codeGen.Type} requires Namespace");
            }

            ValidateTemplateConfig(codeGen);
        }
    }

    private static void ValidateTemplateConfig(CodeGenConfig config)
    {
        if (config.Templates == null)
        {
            throw new ArgumentException($"CodeGen configuration of type {config.Type} requires Templates");
        }

        if (config.Templates.RecordPath.IsNullOrEmpty())
        {
            throw new ArgumentException($"CodeGen configuration of type {config.Type} requires RecordPath template");
        }

        if (config.Templates.TablePath.IsNullOrEmpty())
        {
            throw new ArgumentException($"CodeGen configuration of type {config.Type} requires TablePath template");
        }

        if (config.Templates.SystemPath.IsNullOrEmpty())
        {
            throw new ArgumentException($"CodeGen configuration of type {config.Type} requires SystemPath template");
        }

        var recordTemplatePath = Path.Combine(PathHelper.ProjectRootPath, config.Templates.RecordPath);
        var tableTemplatePath = Path.Combine(PathHelper.ProjectRootPath, config.Templates.TablePath);
        var managerTemplatePath = Path.Combine(PathHelper.ProjectRootPath, config.Templates.SystemPath);

        if (!File.Exists(recordTemplatePath))
        {
            throw new FileNotFoundException($"Record template file not found: {recordTemplatePath}");
        }

        if (!File.Exists(tableTemplatePath))
        {
            throw new FileNotFoundException($"Table template file not found: {tableTemplatePath}");
        }

        if (!File.Exists(managerTemplatePath))
        {
            throw new FileNotFoundException($"Manager template file not found: {managerTemplatePath}");
        }
    }
}
