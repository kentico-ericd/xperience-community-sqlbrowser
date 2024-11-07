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
    public string ExportToCsv()
    {
        string path = GetExportPath(".csv");
        var dynamics = sqlBrowserResultProvider.GetRowsAsDynamic();
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(dynamics);
        }

        return path;
    }


    public string ExportToJson()
    {
        string path = GetExportPath(".json");
        var dynamics = sqlBrowserResultProvider.GetRowsAsDynamic();
        string jsonText = JsonConvert.SerializeObject(dynamics);
        File.WriteAllText(path, jsonText);

        return path;
    }


    public string ExportToXls()
    {
        string path = GetExportPath(".xlsx");
        var workbook = GetWorkbook();
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


    private IWorkbook GetWorkbook()
    {
        var columnNames = sqlBrowserResultProvider.GetColumnNames();
        var dynamics = sqlBrowserResultProvider.GetRowsAsDynamic();

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
            row = excelSheet.CreateRow(rowIndex);
            int cellIndex = 0;
            foreach (string col in columnNames)
            {
                var cell = row.CreateCell(cellIndex);
                object? value = (dyn as IDictionary<string, object>)?[col];
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
