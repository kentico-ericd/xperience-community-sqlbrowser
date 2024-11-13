using CMS.Base;

using CsvHelper;

using Newtonsoft.Json;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System.Globalization;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Default implementation of <see cref="ISqlBrowserExporter"/>.
/// </summary>
public class SqlBrowserExporter(ISqlBrowserResultProvider sqlBrowserResultProvider) : ISqlBrowserExporter
{
    public async Task<string> ExportToCsv()
    {
        string path = GetExportPath(".csv");
        var dynamics = await sqlBrowserResultProvider.GetRowsAsDynamic();
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(dynamics);
        }

        return path;
    }


    public async Task<string> ExportToJson()
    {
        string path = GetExportPath(".json");
        var dynamics = await sqlBrowserResultProvider.GetRowsAsDynamic();
        string jsonText = JsonConvert.SerializeObject(dynamics);
        await File.WriteAllTextAsync(path, jsonText);

        return path;
    }


    public async Task<string> ExportToXls()
    {
        string path = GetExportPath(".xlsx");
        var workbook = await GetWorkbook();
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(fs);
        }

        return path;
    }


    private static string GetExportPath(string extension)
    {
        string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + extension;
        string folder = Path.Combine(SystemContext.WebApplicationPhysicalPath, $"App_Data\\Export");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return Path.Combine(folder, fileName);
    }


    private async Task<IWorkbook> GetWorkbook()
    {
        var columnNames = sqlBrowserResultProvider.GetColumnNames();
        var dynamics = await sqlBrowserResultProvider.GetRowsAsDynamic();

        IWorkbook workbook = new XSSFWorkbook();
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
