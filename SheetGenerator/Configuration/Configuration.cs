namespace SheetGenerator.Configuration;

public class GeneratorSettings
{
    public List<ImportConfig> Imports { get; set; } = new();
    public List<ExportConfig> Exports { get; set; } = new();
    public List<CodeGenConfig> CodeGens { get; set; } = new();
}

#region Import Configurations

public abstract class ImportConfig
{
    protected ImportConfig()
    {
        Type = string.Empty;
    }

    public string Type { get; set; }
}

public class GoogleSheetImportConfig : ImportConfig
{
    public GoogleSheetImportConfig()
    {
        SpreadsheetId = string.Empty;
        Auth = new GoogleAuthConfig();
    }

    public string SpreadsheetId { get; set; }
    public GoogleAuthConfig Auth { get; set; }
}

public class GoogleAuthConfig
{
    public GoogleAuthConfig()
    {
        AuthType = string.Empty;
        ApiKey = string.Empty;
        ServiceAccountPath = string.Empty;
    }

    public string AuthType { get; set; } // "ServiceAccount" 또는 "ApiKey"
    public string ApiKey { get; set; }
    public string ServiceAccountPath { get; set; }
}

public class ExcelImportConfig : ImportConfig
{
    public ExcelImportConfig()
    {
        FilePath = string.Empty;
    }

    public string FilePath { get; set; }
}

#endregion

#region Export Configurations

public abstract class ExportConfig
{
    protected ExportConfig()
    {
        Type = string.Empty;
        OutputPath = string.Empty;
    }

    public string Type { get; set; }
    public string OutputPath { get; set; }
}

public class BinaryExportConfig : ExportConfig
{
    public BinaryExportConfig()
    {
        Extension = string.Empty;
        Compression = string.Empty;
        Encoding = string.Empty;
    }

    public string Extension { get; set; }
    public string Compression { get; set; }
    public string Encoding { get; set; }
}

public class JsonExportConfig : ExportConfig
{
    public JsonExportConfig()
    {
        WriteIndented = true;
    }

    public bool WriteIndented { get; set; } = true;
}

#endregion

#region CodeGen Configurations

public abstract class CodeGenConfig
{
    protected CodeGenConfig()
    {
        Type = string.Empty;
        FileLoadType = string.Empty;
        Namespace = string.Empty;
        OutputPath = string.Empty;
        Templates = new TemplateConfig();
    }

    public string Type { get; set; }
    public string FileLoadType { get; set; }
    public string Namespace { get; set; }
    public TemplateConfig Templates { get; set; }
    public string OutputPath { get; set; }
}

public class CSharpCodeGenConfig : CodeGenConfig
{
}

public class CppCodeGenConfig : CodeGenConfig
{
}

public class TemplateConfig
{
    public TemplateConfig()
    {
        RecordPath = string.Empty;
        TablePath = string.Empty;
        SystemPath = string.Empty;
    }

    public string RecordPath { get; set; }
    public string TablePath { get; set; }
    public string SystemPath { get; set; }
}

#endregion
