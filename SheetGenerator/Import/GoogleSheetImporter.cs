using Serilog;
using SheetGenerator.Configuration;
using SheetGenerator.Models;

namespace SheetGenerator.Import;

public class GoogleSheetImporter : IDataImporter
{
    private const string TableMarker = "@table:";
    private const int HeaderOffset = 2; // 테이블 마커 이후 컬럼명까지의 간격
    private const int DescriptionOffset = 3; // 테이블 마커 Description까지의 간격
    private const int TypeOffset = 4; // 테이블 마커 이후 타입 정의까지의 간격
    private const int DataStartOffset = 5; // 테이블 마커 이후 실제 데이터 시작까지의 간격

    private readonly GoogleSheetImportConfig _config;
    private readonly GoogleSheetService _sheetsService;

    public GoogleSheetImporter(ImportConfig config)
    {
        _config = config as GoogleSheetImportConfig ?? throw new ArgumentNullException(nameof(config));
        _sheetsService = new GoogleSheetService(_config.Auth);
    }

    public async Task<List<Sheet>> ImportAsync(GeneratorSettings config)
    {
        try
        {
            Log.Information("Starting to fetch Google Sheet data: {SpreadsheetId}", _config.SpreadsheetId);

            var spreadsheet = await _sheetsService.Service.Spreadsheets.Get(_config.SpreadsheetId).ExecuteAsync();
            var sheets = new List<Sheet>();

            foreach (var sheetProperties in spreadsheet.Sheets)
            {
                var sheetName = sheetProperties.Properties.Title;
                Log.Information("시트 처리 중: {SheetName}", sheetName);

                var sheet = await ProcessSheet(_config.SpreadsheetId, sheetName);
                sheets.Add(sheet);
            }

            return sheets;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "구글 시트 데이터 가져오기 실패");
            throw;
        }
    }

    private async Task<Sheet> ProcessSheet(string spreadsheetId, string sheetName)
    {
        var range = $"{sheetName}!A1:ZZ";
        var response = await _sheetsService.Service.Spreadsheets.Values.Get(spreadsheetId, range).ExecuteAsync();
        var values = response.Values;

        if (values == null || values.Any() == false)
        {
            return new Sheet { Name = sheetName };
        }

        var sheet = new Sheet { Name = sheetName };
        var tables = GenerateTable(values);

        foreach (var table in tables)
        {
            ProcessTable(values, table);
            sheet.Tables.Add(table);
        }

        return sheet;
    }

    private List<Table> GenerateTable(IList<IList<object>> values)
    {
        var tables = new List<Table>();

        for (var rowIndex = 0; rowIndex < values.Count; rowIndex++)
        {
            var row = values[rowIndex];
            if (row == null)
            {
                continue;
            }

            for (var colIndex = 0; colIndex < row.Count; colIndex++)
            {
                var cellValue = row[colIndex]?.ToString();
                if (string.IsNullOrEmpty(cellValue))
                {
                    continue;
                }

                if (cellValue.StartsWith(TableMarker))
                {
                    if (CreateTable(values, rowIndex, colIndex, cellValue) is var table)
                    {
                        tables.Add(table);
                    }
                }
            }
        }

        return tables;
    }

    private void ProcessTable(IList<IList<object>> values, Table table)
    {
        var headerRowIndex = table.StartRowIndex + HeaderOffset;
        var typeRowIndex = table.StartRowIndex + TypeOffset;
        var descriptionRowIndex = table.StartRowIndex + DescriptionOffset;

        var columns = GetTableColumns(values, headerRowIndex, typeRowIndex, descriptionRowIndex, table.StartColumnIndex);
        table.Columns.AddRange(columns);

        var endRow = GetTableEndRow(values, table);
        ProcessTableData(values, table, endRow);
    }

    private List<Column> GetTableColumns(IList<IList<object>> values,
            int headerRowIndex, int typeRowIndex, int descriptionRowIndex, int startCol)
    {
        var columns = new List<Column>();
        if (headerRowIndex >= values.Count)
        {
            return columns;
        }

        var headerRow = values[headerRowIndex];
        var typeRow = values[typeRowIndex];
        var descriptionRow = descriptionRowIndex < values.Count ? values[descriptionRowIndex] : null;

        var columnIndex = 0;

        for (var i = startCol; i < headerRow.Count; i++)
        {
            var columnName = headerRow[i]?.ToString();
            if (string.IsNullOrEmpty(columnName))
            {
                break;
            }

            var columnType = GetColumnType(typeRow, i);
            var columnDesc = GetColumnDescription(descriptionRow, i);

            columns.Add(new Column { Name = columnName, Type = columnType, Index = columnIndex++, Description = columnDesc });

            Log.Debug("Added column: {ColumnName}, Type: {ColumnType}, Index: {Index}",
                    columnName, columnType, columnIndex - 1);
        }

        return columns;
    }


    private string GetColumnDescription(IList<object>? descriptionRow, int columnIndex)
    {
        if (descriptionRow != null && columnIndex < descriptionRow.Count)
        {
            return descriptionRow[columnIndex]?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }

    private void ProcessTableData(IList<IList<object>> values, Table table, int endRow)
    {
        var dataStartRow = table.StartRowIndex + DataStartOffset;

        for (var rowIndex = dataStartRow; rowIndex < endRow; rowIndex++)
        {
            if (rowIndex >= values.Count)
            {
                break;
            }

            var rowData = values[rowIndex];
            if (rowData == null || rowData.Count <= table.StartColumnIndex)
            {
                continue;
            }

            var row = new Row { Index = rowIndex - dataStartRow, Cells = new List<Cell>(table.Columns.Count) };

            foreach (var column in table.Columns)
            {
                var cellIndex = table.StartColumnIndex + column.Index;
                if (cellIndex < rowData.Count)
                {
                    row.Cells.Add(new Cell { Value = rowData[cellIndex], ColumnIndex = column.Index });
                }
            }

            table.Rows.Add(row);
        }
    }

    private static int GetTableEndRow(IList<IList<object>> values, Table table)
    {
        var dataStartRow = table.StartRowIndex + DataStartOffset;

        for (var rowIndex = dataStartRow; rowIndex < values.Count; rowIndex++)
        {
            var row = values[rowIndex];
            if (row == null || !row.Any())
            {
                if (IsEmptyRow(values, rowIndex + 1))
                {
                    return rowIndex;
                }

                continue;
            }

            if (HasTableMarker(row))
            {
                return rowIndex;
            }
        }

        return values.Count;
    }

    private static bool IsEmptyRow(IList<IList<object>> values, int rowIndex)
    {
        return rowIndex < values.Count &&
               (values[rowIndex] == null || !values[rowIndex].Any() ||
                values[rowIndex].All(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())));
    }

    private static bool HasTableMarker(IList<object> row)
    {
        return row.Any(cell => cell?.ToString()?.StartsWith(TableMarker) == true);
    }

    private static string GetColumnType(IList<object> typeRow, int colIndex)
    {
        return colIndex < typeRow?.Count ? typeRow[colIndex]?.ToString() ?? "string" : "string";
    }

    private Table CreateTable(IList<IList<object>> values, int rowIndex, int colIndex, string cellValue)
    {
        var tableName = cellValue.Substring(TableMarker.Length).Trim();
        var nextRowIndex = rowIndex + 1;
        var descriptionRow = colIndex < values[nextRowIndex]?.Count ? values[nextRowIndex] : default;
        var description = descriptionRow?[colIndex]?.ToString() ?? string.Empty;

        var newTable = new Table
        {
                Name = tableName,
                Description = description,
                StartRowIndex = rowIndex,
                StartColumnIndex = colIndex,
                Columns = new List<Column>(),
                Rows = new List<Row>()
        };

        return newTable;
    }
}
