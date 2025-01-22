using System.Text;
using SheetGenerator.CodeGenerate;
using SheetGenerator.Configuration;
using SheetGenerator.Models;
using SheetGenerator.Util;

public class CSharpCodeGenerator : ICodeGenerator
{
    private readonly CodeGenConfig _codeGenConfig;
    private readonly string _managerTemplate;
    private readonly string _recordTemplate;
    private readonly string _tableTemplate;

    public CSharpCodeGenerator(CodeGenConfig config)
    {
        _codeGenConfig = config;

        var recordTemplatePath = Path.Combine(PathHelper.ProjectRootPath, config.Templates.RecordPath);
        var tableTemplatePath = Path.Combine(PathHelper.ProjectRootPath, config.Templates.TablePath);
        var managerTemplatePath = Path.Combine(PathHelper.ProjectRootPath, config.Templates.SystemPath);

        _recordTemplate = File.ReadAllText(recordTemplatePath);
        _tableTemplate = File.ReadAllText(tableTemplatePath);
        _managerTemplate = File.ReadAllText(managerTemplatePath);
    }

    public async Task GenerateAsync(List<Sheet> sheets, string outputPath)
    {
        var allTables = sheets.SelectMany(s => s.Tables).ToList();

        foreach (var sheet in sheets)
        {
            foreach (var table in sheet.Tables)
            {
                await GenerateRecordClass(table, outputPath);
                await GenerateTableClass(table, outputPath);
            }
        }

        await GenerateTableSystem(allTables, outputPath);
    }

    private async Task GenerateRecordClass(Table table, string outputPath)
    {
        var code = GenerateRecordContent(table);

        var filePath = Path.Combine(outputPath, $"{table.Name}Record.cs");
        var directoryPath = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await File.WriteAllTextAsync(filePath, code);
    }

    private string GenerateRecordContent(Table table)
    {
        var properties = new StringBuilder();
        var readLogic = new StringBuilder();
        var toStringContent = new StringBuilder();
        var equalityContent = new StringBuilder();
        var hashContent = new StringBuilder();
        var fieldInfoContent = new StringBuilder();
        var indexedFieldsContent = new StringBuilder();
        var copyFromContent = new StringBuilder();

        foreach (var column in table.Columns)
        {
            // Properties - MessagePack 속성 추가
            properties.AppendLine($"        /// <summary>{column.Description}</summary>");
            properties.AppendLine($"        [Key({column.Index})]");
            properties.AppendLine($"        public {GetCSharpType(column.Type)} {column.Name} {{ get; private set; }}");
            properties.AppendLine();

            copyFromContent.AppendLine($"            {column.Name} = source.{column.Name};");

            // ReadLogic - MessagePack 역직렬화 로직으로 변경
            readLogic.AppendLine($"                    case \"{column.Name}\":");
            readLogic.AppendLine($"                        {column.Name} = ({GetCSharpType(column.Type)})data[\"{column.Name}\"];");
            readLogic.AppendLine("                        break;");

            toStringContent.AppendLine($"            sb.Append(\"\\\"{column.Name}\\\":\");");
            toStringContent.AppendLine($"            StringFormatter.Format({column.Name}, sb);");

            var type = GetCSharpType(column.Type);
            if (type == "string")
            {
                equalityContent.AppendLine($"            if (!string.Equals({column.Name}, other.{column.Name}, StringComparison.Ordinal)) return false;");
            }
            else
            {
                equalityContent.AppendLine($"            if (!{column.Name}.Equals(other.{column.Name})) return false;");
            }

            hashContent.AppendLine($"            hash.Add({column.Name});");

            fieldInfoContent.AppendLine($"                [\"{column.Name}\"] = new FieldInfo({column.Index}, record => record.{column.Name}),");
            indexedFieldsContent.AppendLine($"                new FieldInfo({column.Index}, record => record.{column.Name}),");
        }

        equalityContent.AppendLine("            return true;");

        return _recordTemplate
               .Replace("{Namespace}", _codeGenConfig.Namespace)
               .Replace("{TableName}", table.Name)
               .Replace("{Properties}", properties.ToString().TrimEnd())
               .Replace("{ToStringContent}", toStringContent.ToString())
               .Replace("{EqualityContent}", equalityContent.ToString())
               .Replace("{HashContent}", hashContent.ToString())
               .Replace("{FieldInfoContent}", fieldInfoContent.ToString().TrimEnd())
               .Replace("{IndexedFieldsContent}", indexedFieldsContent.ToString().TrimEnd())
               .Replace("{CopyFromContent}", copyFromContent.ToString().TrimEnd());
    }


    private async Task GenerateTableClass(Table table, string outputPath)
    {
        var code = GenerateTableContent(table);

        var filePath = Path.Combine(outputPath, $"{table.Name}Table.cs");
        var directoryPath = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await File.WriteAllTextAsync(filePath, code);
    }

    private string GenerateTableContent(Table table)
    {
        var fieldNames = string.Join("\", \"", table.Columns.Select(c => c.Name));
        fieldNames = $"\"{fieldNames}\"";

        var buildObjectMapParams = string.Join(", ", table.Columns.Select(c => $"record.{c.Name}"));
        var indexingContent = GenerateIndexingContent(table);
        var indexMapping = GenerateIndexMapping(table);

        return _tableTemplate
               .Replace("{Namespace}", _codeGenConfig.Namespace)
               .Replace("{TableName}", table.Name)
               .Replace("{TableDescription}", table.Description ?? $"Table for {table.Name}")
               .Replace("{FieldNames}", fieldNames)
               .Replace("{BuildObjectMapParams}", buildObjectMapParams)
               .Replace("{IndexingContent}", indexingContent)
               .Replace("{IndexMapping}", indexMapping);
    }


    private string GenerateIndexingContent(Table table)
    {
        var sb = new StringBuilder();

        if (table.Columns.Any(c => c.Name == "Index"))
        {
            sb.AppendLine($"        public Dictionary<int, {table.Name}Record> RecordsByIndex => _recordsByIndex;");
            sb.AppendLine($"        private readonly Dictionary<int, {table.Name}Record> _recordsByIndex = new();");
            sb.AppendLine();
            sb.AppendLine($"        public {table.Name}Record GetByIndex(int index)"); // 반환 타입도 변경
            sb.AppendLine("        {");
            sb.AppendLine("            if (_recordsByIndex.TryGetValue(index, out var record))");
            sb.AppendLine("                return record;");
            sb.AppendLine("            throw new KeyNotFoundException($\"Record with index {index} not found.\");");
            sb.AppendLine("        }");
        }

        if (table.Columns.Any(c => c.Name == "Key"))
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine($"        public Dictionary<string, {table.Name}Record> RecordsByKey => _recordsByKey;");
            sb.AppendLine($"        private readonly Dictionary<string, {table.Name}Record> _recordsByKey = new();");
            sb.AppendLine();
            sb.AppendLine($"        public {table.Name}Record GetByKey(string key)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_recordsByKey.TryGetValue(key, out var record))");
            sb.AppendLine("                return record;");
            sb.AppendLine("            throw new KeyNotFoundException($\"Record with key {key} not found.\");");
            sb.AppendLine("        }");
        }

        return sb.ToString();
    }

    private string GenerateIndexMapping(Table table)
    {
        var sb = new StringBuilder();

        if (table.Columns.Any(c => c.Name == "Index"))
        {
            sb.AppendLine("                _recordsByIndex.Add(newRecord.Index, newRecord);");
        }

        if (table.Columns.Any(c => c.Name == "Key"))
        {
            sb.AppendLine("                _recordsByKey.Add(newRecord.Key, newRecord);");
        }

        return sb.ToString();
    }

    private async Task GenerateTableSystem(List<Table> tables, string outputPath)
    {
        var tableDeclarations = new StringBuilder();
        var propertyDeclarations = new StringBuilder();
        var initializeContent = new StringBuilder();
        var releaseContent = new StringBuilder();

        var fileExtention = _codeGenConfig.FileLoadType.ToLower().Equals("binary") ? "table" : "json";

        foreach (var table in tables)
        {
            tableDeclarations.AppendLine($"        private static {table.Name}Table _{table.Name.ToCamelCase()};");

            propertyDeclarations.AppendLine($"        public static {table.Name}Table {table.Name} {{ get; set; }}");

            initializeContent.AppendLine($"                    InitializeTableAsync<{table.Name}Table>(basePath, \"{table.Name}Data.{fileExtention}\", table => {table.Name} = table),");

            releaseContent.AppendLine($"            {table.Name}?.Clear();");
            releaseContent.AppendLine($"            {table.Name} = null;");
        }

        var code = _managerTemplate
                   .Replace("{Namespace}", _codeGenConfig.Namespace)
                   .Replace("{Properties}", propertyDeclarations.ToString().TrimEnd())
                   .Replace("{InitializeContent}", initializeContent.ToString().TrimEnd())
                   .Replace("{ReleaseContent}", releaseContent.ToString().TrimEnd());

        var filePath = Path.Combine(outputPath, "TableSystem.cs");
        var directoryPath = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await File.WriteAllTextAsync(filePath, code);
    }

    private string GetCSharpType(string columnType)
    {
        return columnType.ToLower() switch
        {
                "int" => "int",
                "float" => "float",
                "double" => "double",
                "bool" => "bool",
                "string" => "string",
                "datetime" => "DateTime",
                _ => "string"
        };
    }
}
