using System.Text.Json;
using Serilog;
using SheetGenerator.Configuration;

namespace SheetGenerator;

public static class EnvironmentInitializer
{
    private static readonly string _readMeContent = @"# Credentials Setup Guide

1. Rename `service_account.template.json` to `service_account.json`
2. Fill in the required information from Google Cloud Console:
  - Go to 'IAM & Admin' > 'Service Accounts'
  - Create a new service account or select existing one
  - Create/Add key (JSON type)
  - Copy the contents to `service_account.json`

Note: Never commit `service_account.json` to version control!
";

    public static void Initialize(bool force = false)
    {
        var installationPath = Environment.ProcessPath != null
                ? Path.GetDirectoryName(Environment.ProcessPath)
                : AppContext.BaseDirectory;

        if (string.IsNullOrEmpty(installationPath))
        {
            throw new InvalidOperationException("Cannot determine installation path.");
        }

        Log.Information($"Installation path: {installationPath}");

        CreateSettingsFile(installationPath, force);
        CreateTemplateFiles(installationPath, force);
        CreateServiceAccountTemplate(installationPath, force);

        Log.Information("Environment initialization completed successfully.");
    }

    private static void CreateSettingsFile(string basePath, bool force)
    {
        var settingsPath = Path.Combine(basePath, "settings.json");
        Log.Information($"Creating settings file at: {settingsPath}");

        if (File.Exists(settingsPath) && !force)
        {
            Log.Warning("Settings file already exists. Use --force to overwrite.");
            return;
        }

        var generatorSettings = new GeneratorSettings
        {
                Imports =
                        new List<ImportConfig>
                        {
                                new GoogleSheetImportConfig
                                {
                                        Type = "GoogleSheet", SpreadsheetId = "", Auth = new GoogleAuthConfig { AuthType = "ServiceAccount", ApiKey = "", ServiceAccountPath = "credentials/service_account.json" }
                                }
                        },
                Exports = new List<ExportConfig>
                {
                        new BinaryExportConfig
                        {
                                Type = "Binary",
                                OutputPath = "Generated/Data",
                                Extension = "table",
                                Compression = "none",
                                Encoding = "utf8"
                        }
                },
                CodeGens = new List<CodeGenConfig>
                {
                        new CSharpCodeGenConfig
                        {
                                Type = "CSharp",
                                FileLoadType = "binary",
                                Namespace = "StaticData.Tables",
                                OutputPath = "Generated/Script/CSharp",
                                Templates = new TemplateConfig { RecordPath = "ScriptTemplate/CSharp/RecordTemplate.txt", TablePath = "ScriptTemplate/CSharp/TableTemplate.txt", SystemPath = "ScriptTemplate/CSharp/SystemTemplate.txt" }
                        }
                }
        };

        var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new ImportConfigConverter(), new ExportConfigConverter(), new CodeGenConfigConverter() } };

        var json = JsonSerializer.Serialize(generatorSettings, options);
        File.WriteAllText(settingsPath, json);
        Log.Information("Created settings.json");
    }

    private static void CreateTemplateFiles(string basePath, bool force)
    {
        var targetTemplateDir = Path.Combine(basePath, "ScriptTemplate");

        foreach (var templateName in TemplateResource.GetTemplateNames())
        {
            var templateContent = TemplateResource.GetTemplate(templateName);
            var templatePath = Path.Combine(targetTemplateDir, templateName + ".txt");
            var templateDir = Path.GetDirectoryName(templatePath);

            if (!Directory.Exists(templateDir))
            {
                Directory.CreateDirectory(templateDir);
            }

            Log.Information($"CreateTemplateFiles templatePath = {templatePath}");

            if (File.Exists(templatePath) && !force)
            {
                Log.Warning($"Template file {templateName} already exists. Use --force to overwrite.");
                continue;
            }

            File.WriteAllText(templatePath, templateContent);
            Log.Information($"Created template file: {templateName}");
        }
    }

    private static void CreateServiceAccountTemplate(string basePath, bool force)
    {
        var credentialsDir = Path.Combine(basePath, "credentials");
        Directory.CreateDirectory(credentialsDir);

        var templatePath = Path.Combine(credentialsDir, "service_account.template.json");
        Log.Information($"CreateServiceAccountTemplate templatePath = {templatePath}");

        if (File.Exists(templatePath) && !force)
        {
            Log.Warning("Service account template file already exists. Use --force to overwrite.");
            return;
        }

        var templateContent = new Dictionary<string, object>
        {
                ["type"] = "service_account",
                ["project_id"] = "",
                ["private_key_id"] = "",
                ["private_key"] = "",
                ["client_email"] = "",
                ["client_id"] = "",
                ["auth_uri"] = "https://accounts.google.com/o/oauth2/auth",
                ["token_uri"] = "https://oauth2.googleapis.com/token",
                ["auth_provider_x509_cert_url"] = "https://www.googleapis.com/oauth2/v1/certs",
                ["client_x509_cert_url"] = ""
        };

        var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(templateContent, options);
        File.WriteAllText(templatePath, json);

        var readmePath = Path.Combine(credentialsDir, "README.md");
        File.WriteAllText(readmePath, _readMeContent);
        Log.Information("Created service account template and setup guide");
    }
}
