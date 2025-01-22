using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Serilog;
using SheetGenerator.Configuration;
using SheetGenerator.Util;

namespace SheetGenerator;

public class GoogleSheetService
{
    private const string ApplicationName = "SheetGenerator";
    private const string CredentialPath = "service_account.json";
    private readonly GoogleAuthConfig _authConfig;
    private readonly string _spreadsheetId = "Sheet Generator";
    private readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

    public GoogleSheetService(GoogleAuthConfig authConfig)
    {
        _authConfig = authConfig;
        Service = CreateSheetsService();
    }

    public SheetsService Service { get; }


    public SheetsService CreateSheetsService()
    {
        try
        {
            return _authConfig.AuthType.ToLower() switch
            {
                    "apikey" => new SheetsService(new BaseClientService.Initializer { ApiKey = _authConfig.ApiKey, ApplicationName = ApplicationName }),

                    "serviceaccount" => CreateServiceWithServiceAccount(_authConfig.ServiceAccountPath),

                    _ => throw new ArgumentException($"Unsupported authentication type: {_authConfig.AuthType}")
            };
        }
        catch (FileNotFoundException ex)
        {
            throw new Exception($"[Exception] InitializeAsync() {CredentialPath} File not found.");
        }
        catch (GoogleApiException ex)
        {
            throw new Exception($"[Exception] InitializeAsync() Google API Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"[Exception] InitializeAsync() Error during initialization: {ex.Message}");
        }
    }

    private SheetsService CreateServiceWithServiceAccount(string serviceAccountPath)
    {
        try
        {
            var path = Path.GetFullPath(Path.Combine(PathHelper.ProjectRootPath, serviceAccountPath));
            Log.Information($"CreateServiceWithServiceAccount absolute path = {path}");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Service account file not found: {path}");
            }

            // FromFile 사용
            var credential = GoogleCredential
                             .FromFile(path) // FromStream 대신 FromFile 사용
                             .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            Log.Information("Google credential created successfully");

            return new SheetsService(new BaseClientService.Initializer { HttpClientInitializer = credential, ApplicationName = ApplicationName });
        }
        catch (Exception ex)
        {
            Log.Error($"Error in CreateServiceWithServiceAccount: {ex.Message}");
            if (ex.InnerException != null)
            {
                Log.Error($"Inner exception: {ex.InnerException.Message}");
                Log.Error($"Inner exception stack trace: {ex.InnerException.StackTrace}");
            }

            throw;
        }
    }

    public async Task<IList<IList<object>>> GetValuesAsync(string spreadsheetId, string sheetName)
    {
        var dimensions = await GetSheetDimensionsAsync(spreadsheetId, sheetName);

        var lastColumn = ConvertToA1Notation(dimensions.LastColumn);
        var range = $"{sheetName}!A1:{lastColumn}{dimensions.LastRow}";

        var request = Service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();
        return response.Values;
    }

    private string ConvertToA1Notation(int columnNumber)
    {
        var columnName = "";
        while (columnNumber > 0)
        {
            var remainder = (columnNumber - 1) % 26;
            columnName = (char)('A' + remainder) + columnName;
            columnNumber = (columnNumber - 1) / 26;
        }

        return columnName;
    }

    public async Task<(int LastRow, int LastColumn)> GetSheetDimensionsAsync(string spreadsheetId, string sheetName)
    {
        var response = await Service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
        var sheet = response.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);

        if (sheet == null)
        {
            throw new InvalidOperationException($"Sheet '{sheetName}' not found.");
        }

        return (
                LastRow: sheet.Properties.GridProperties.RowCount ?? 0,
                LastColumn: sheet.Properties.GridProperties.ColumnCount ?? 0
        );
    }

    public async Task<IList<IList<object>>> GetValuesInBatchesAsync(string spreadsheetId,
            string sheetName,
            int batchSize = 1000)
    {
        var dimensions = await GetSheetDimensionsAsync(spreadsheetId, sheetName);
        var allValues = new List<IList<object>>();

        for (var startRow = 1; startRow <= dimensions.LastRow; startRow += batchSize)
        {
            var endRow = Math.Min(startRow + batchSize - 1, dimensions.LastRow);
            var lastColumn = ConvertToA1Notation(dimensions.LastColumn);
            var range = $"{sheetName}!A{startRow}:{lastColumn}{endRow}";

            var batchValues = await Service.Spreadsheets.Values
                                           .Get(spreadsheetId, range)
                                           .ExecuteAsync();

            if (batchValues.Values != null)
            {
                allValues.AddRange(batchValues.Values);
            }
        }

        return allValues;
    }

    public async Task<IList<string>> GetSheetNamesAsync(string spreadsheetId)
    {
        try
        {
            var response = await Service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();

            foreach (var sheet in response.Sheets)
            {
            }


            return response.Sheets.Select(s => s.Properties.Title).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"[Exception] GetSheetNamesAsync() Error Occurred: {ex.Message}");
        }
    }
}
