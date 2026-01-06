using CMS.Base;

using CsvHelper;

using Newtonsoft.Json;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System.Globalization;

using XperienceCommunity.SqlBrowser.Enum;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Default implementation of <see cref="ISqlBrowserExporter"/>.
/// </summary>
public class SqlBrowserExporter(ISqlBrowserResultProvider sqlBrowserResultProvider) : ISqlBrowserExporter
{
    private const string EXPORT_DIRECTORY = "App_Data\\Export";


    public async Task<string> Export(SqlBrowserExportType exportType, string? fileName = null)
    {
        string path = GetExportPath(exportType, fileName);
        switch (exportType)
        {
            case SqlBrowserExportType.Excel:
                await ExportToXls(path);
                break;
            case SqlBrowserExportType.Csv:
                await ExportToCsv(path);
                break;
            case SqlBrowserExportType.Json:
                await ExportToJson(path);
                break;
            default:
            case SqlBrowserExportType.None:
                throw new InvalidOperationException();
        }

        return path;
    }


    private async Task ExportToCsv(string path)
    {
        var dynamics = await sqlBrowserResultProvider.GetRowsAsDynamic();
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(dynamics);
    }


    private async Task ExportToJson(string path)
    {
        var dynamics = await sqlBrowserResultProvider.GetRowsAsDynamic();
        string jsonText = JsonConvert.SerializeObject(dynamics);
        await File.WriteAllTextAsync(path, jsonText);
    }


    private async Task ExportToXls(string path)
    {
        var workbook = await GetWorkbook();
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        workbook.Write(fs);
    }


    public string GetExportDirectory() => Path.Combine(SystemContext.WebApplicationPhysicalPath, EXPORT_DIRECTORY);


    private string GetExportPath(SqlBrowserExportType exportType, string? fileName)
    {
        string extension = exportType switch
        {
            SqlBrowserExportType.Csv => ".csv",
            SqlBrowserExportType.Excel => ".xlsx",
            SqlBrowserExportType.Json => ".json",
            SqlBrowserExportType.None => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };

        string fullName = (fileName ?? DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")) + extension;
        string folder = GetExportDirectory();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return Path.Combine(folder, fullName);
    }


    private async Task<IWorkbook> GetWorkbook()
    {
        var columnNames = sqlBrowserResultProvider.GetColumnNames();
        var dynamics = await sqlBrowserResultProvider.GetRowsAsDynamic();

        var workbook = new XSSFWorkbook();
        var excelSheet = workbook.CreateSheet("Sheet1");

        var row = excelSheet.CreateRow(0);
        int columnIndex = 0;
        foreach (string col in columnNames)
        {
            row.CreateCell(columnIndex).SetCellValue(col);
            columnIndex++;
        }

        int rowIndex = 1;
        foreach (var dyn in dynamics)
        {
            int cellIndex = 0;
            row = excelSheet.CreateRow(rowIndex);
            var valuesDictionary = dyn as IDictionary<string, object>;
            foreach (string col in columnNames)
            {
                var cell = row.CreateCell(cellIndex);
                object? value = valuesDictionary?[col];
                if (value is not null)
                {
                    cell.SetCellValue(value.ToString());
                }

                cellIndex++;
            }

            rowIndex++;
        }

        return workbook;
    }
}
