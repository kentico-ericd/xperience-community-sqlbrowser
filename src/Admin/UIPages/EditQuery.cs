using System.Data;
using System.Text;
using System.Text.RegularExpressions;

using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;

using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Edit UI page for submitting query text to <see cref="ISqlBrowserResultProvider"/>. 
/// </summary>
[UINavigation(false)]
public class EditQuery(
    IFormDataBinder binder,
    IFormComponentMapper formComponentMapper,
    IPageUrlGenerator pageUrlGenerator,
    ISqlBrowserResultProvider sqlBrowserResultProvider,
    IProgressiveCache progressiveCache,
    IEventLogService eventLogService) : EditPageBase(binder)
{
    private const string QUERY_FIELDNAME = "QueryText";
    private const string TABLES_FIELDNAME = "DatabaseTables";
    private const string EOL_REPLACEMENT = "#EOL#";
    private readonly Regex newLineRegex = RegexHelper.GetRegex(@"(<br[ ]?/>)|([\r]?\n)");


    public override async Task<EditTemplateClientProperties> ConfigureTemplateProperties(EditTemplateClientProperties properties)
    {
        properties.SubmitButton.Label = "Run";
        properties.Callouts.Add(new()
        {
            Headline = "Enter your SQL query",
            Content = @"Note: SQL browser currently only supports single table results. If your query returns multiple tables, only
                the first table will be displayed.",
            Type = CalloutType.QuickTip
        });
        properties.Items = await GetFormItemsClientProperties() as ICollection<IFormItemClientProperties>;

        return properties;
    }


    protected override Task<ICollection<IFormItem>> GetFormItems()
    {
        var formInfo = GetFormInfo();
        var elements = formInfo.GetFormElements(true, false);
        var formItems = GetFormComponents(elements.OfType<FormFieldInfo>()).ToList<IFormItem>();

        // Remove validation rules from tables component
        var tableComponent = formItems.OfType<TextWithLabelComponent>().FirstOrDefault(prop =>
            prop.Name.Equals(TABLES_FIELDNAME, StringComparison.OrdinalIgnoreCase));
        tableComponent?.ValidationRules.Clear();

        return Task.FromResult(formItems as ICollection<IFormItem>);
    }


    protected override async Task<IEnumerable<IFormItemClientProperties>> GetFormItemsClientProperties()
    {
        var items = await GetFormItems();
        var components = items.OfType<IFormComponent>().ToList();
        await BindContextToComponents(components);
        var properties = await items.OnlyVisible().GetClientProperties();

        // Set properties for table component
        var tableComponentProperties = properties.OfType<TextWithLabelClientProperties>().FirstOrDefault(prop =>
            prop.Name.Equals(TABLES_FIELDNAME, StringComparison.OrdinalIgnoreCase));
        if (tableComponentProperties is not null)
        {
            tableComponentProperties.ValueAsHtml = true;
        }

        return properties;
    }


    protected override Task<ICommandResponse> SubmitInternal(
        FormSubmissionCommandArguments args,
        ICollection<IFormItem> items,
        IFormFieldValueProvider formFieldValueProvider)
    {
        if (formFieldValueProvider.TryGet(QUERY_FIELDNAME, out string queryText))
        {
            sqlBrowserResultProvider.SetQuery(queryText);
            string navigationUrl = pageUrlGenerator.GenerateUrl<ResultListing>();

            return Task.FromResult((ICommandResponse)NavigateTo(navigationUrl));
        }

        return Task.FromResult(Response().AddErrorMessage("Failed to retrieve query text from submission"));
    }


    protected ICollection<IFormComponent> GetFormComponents(IEnumerable<FormFieldInfo> formFields) => formComponentMapper.Map(formFields).ToList();


    private FormInfo GetFormInfo()
    {
        var formInfo = new FormInfo();
        formInfo.AddFormItem(new FormFieldInfo
        {
            Name = QUERY_FIELDNAME,
            AllowEmpty = false,
            Visible = true,
            IsDummyField = true,
            Precision = DataTypeManager.GetDataType(TypeEnum.Field, FieldDataType.LongText).DefaultPrecision,
            DataType = FieldDataType.LongText,
            Enabled = true,
            Settings = new() {
                {"controlname", TextAreaComponent.IDENTIFIER},
                {nameof(TextAreaClientProperties.CopyButtonVisible), false},
                {nameof(TextAreaClientProperties.MaxRowsNumber), 40},
                {nameof(TextAreaClientProperties.MinRowsNumber), 10}
            },
            DefaultValue = sqlBrowserResultProvider.GetQuery() ?? string.Empty
        });

        string tableText = GetTables();
        if (!string.IsNullOrEmpty(tableText))
        {
            formInfo.AddFormItem(new FormFieldInfo
            {
                Name = TABLES_FIELDNAME,
                AllowEmpty = true,
                Visible = true,
                IsDummyField = true,
                Precision = DataTypeManager.GetDataType(TypeEnum.Field, FieldDataType.Text).DefaultPrecision,
                DataType = FieldDataType.Text,
                Enabled = true,
                Settings = new() {
                    {"controlname", TextWithLabelComponent.IDENTIFIER},
                    {nameof(TextWithLabelClientProperties.Disabled), true}
                },
                DefaultValue = tableText
            });
        }

        return formInfo;
    }


    private string GetTables()
    {
        var groupedRows = progressiveCache.Load(LoadTables, new CacheSettings(10, $"{nameof(EditQuery)}|{nameof(GetTables)}"));
        var builder = new StringBuilder()
            .Append("<h2>Available tables</h2>")
            .Append(Environment.NewLine);
        if (!groupedRows.Any())
        {
            return builder.Append("Failed to load database tables, please check the Event log").ToString();
        }

        foreach (var group in groupedRows)
        {
            var columnNames = group.Select(row => row["column"]);
            builder
                .Append("<u>")
                .Append(group.Key)
                .Append("</u>")
                .Append(Environment.NewLine)
                .Append(string.Join(", ", columnNames))
                .Append(Environment.NewLine)
                .Append(Environment.NewLine);
        }

        string resultText = newLineRegex.Replace(builder.ToString(), EOL_REPLACEMENT);

        return resultText.Replace(EOL_REPLACEMENT, "<br />");
    }


    /// <summary>
    /// Gets all database columns grouped by their table.
    /// </summary>
    private IEnumerable<IGrouping<object, DataRow>> LoadTables(CacheSettings cs)
    {
        try
        {
            string query = @"SELECT
                         T.name AS 'table',
                         C.name AS 'column'
                FROM     sys.objects AS T
                         JOIN sys.columns AS C ON T.object_id = C.object_id
                WHERE    T.type = 'U'
                ORDER BY T.name ASC";
            var result = ConnectionHelper.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery);
            if (result.Tables.Count == 0)
            {
                cs.Cached = false;

                return [];
            }

            return result.Tables[0].Rows.OfType<DataRow>().GroupBy(r => r["table"]);
        }
        catch (Exception ex)
        {
            cs.Cached = false;
            eventLogService.LogException(nameof(EditQuery), nameof(LoadTables), ex);

            return [];
        }
    }
}
