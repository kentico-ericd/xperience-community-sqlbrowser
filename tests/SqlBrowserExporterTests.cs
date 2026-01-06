using CMS.Base;

using CsvHelper;

using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NPOI.SS.UserModel;

using NSubstitute;

using XperienceCommunity.SqlBrowser.Services;
using XperienceCommunity.SqlBrowser.Enum;

namespace XperienceCommunity.SqlBrowser.Tests;

public class SqlBrowserExporterTests
{
    private SqlBrowserExporter exporter;
    private readonly string[] columns = ["UserId", "UserName"];
    private readonly List<Dictionary<string, object>> rows =
    [
        new() { { "UserId", 1 }, { "UserName", "a" } },
        new() { { "UserId", 2 }, { "UserName", "b" } }
    ];


    [SetUp]
    public void SetUp()
    {
        var resultProvider = Substitute.For<ISqlBrowserResultProvider>();
        var dynamics = rows.Select(row =>
        {
            var obj = new ExpandoObject();
            foreach (string col in columns)
            {
                (obj as IDictionary<string, object>).Add(col, row[col]);
            }

            return obj;
        });
        resultProvider.GetRowsAsDynamic().Returns(dynamics);
        resultProvider.GetColumnNames().Returns(columns);

        exporter = new(resultProvider);
    }


    [TearDown]
    public void TearDown()
    {
        string exportFolder = Path.Combine(SystemContext.WebApplicationPhysicalPath, $"App_Data\\Export");
        if (Directory.Exists(exportFolder))
        {
            var files = Directory.EnumerateFiles(exportFolder);
            foreach (string path in files)
            {
                File.Delete(path);
            }

            Directory.Delete(exportFolder);
        }
    }


    [Test]
    public async Task Export_InvalidType_Throws() =>
        Assert.ThrowsAsync<InvalidOperationException>(() => exporter.Export(SqlBrowserExportType.None));


    [Test]
    public async Task Export_CustomName_CreatesFileWithName()
    {
        string fileName = "my-file-name";
        string path = await exporter.Export(SqlBrowserExportType.Csv, fileName);

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(path));
            Assert.That(path, Does.EndWith($"{fileName}.csv"));
        });
    }


    [Test]
    public async Task Export_Csv_WritesFileWithData()
    {
        string path = await exporter.Export(SqlBrowserExportType.Csv);

        string name1, name2;
        using (TextReader fileReader = File.OpenText(path))
        {
            var culture = new CultureInfo(name: "en-US", useUserOverride: false);
            var csv = new CsvReader(fileReader, culture);
            var records = csv.GetRecords<dynamic>();
            name1 = records.First().UserName;
            name2 = records.Last().UserName;
        }

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(path));
            Assert.That(name1, Is.EqualTo("a"));
            Assert.That(name2, Is.EqualTo("b"));
        });
    }


    [Test]
    public async Task Export_Json_WritesFileWithData()
    {
        string path = await exporter.Export(SqlBrowserExportType.Json);

        string contents = await File.ReadAllTextAsync(path, Encoding.UTF8);
        var json = JsonConvert.DeserializeObject<JArray>(contents);
        string? name1 = json?[0].Value<string>("UserName");
        string? name2 = json?[1].Value<string>("UserName");

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(path));
            Assert.That(json, Is.Not.Null);
            Assert.That(name1, Is.EqualTo("a"));
            Assert.That(name2, Is.EqualTo("b"));
        });
    }


    [Test]
    public async Task Export_Xls_WritesFileWithData()
    {
        string path = await exporter.Export(SqlBrowserExportType.Excel);

        IWorkbook workbook;
        using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            workbook = WorkbookFactory.Create(file);
        }

        var sheet = workbook.GetSheetAt(0);
        string? name1 = sheet.GetRow(1).GetCell(1).StringCellValue;
        string? name2 = sheet.GetRow(2).GetCell(1).StringCellValue;

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(path));
            Assert.That(name1, Is.EqualTo("a"));
            Assert.That(name2, Is.EqualTo("b"));
        });
    }
}
