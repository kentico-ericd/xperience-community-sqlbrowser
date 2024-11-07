using CMS.DataEngine;
using CMS.FormEngine;

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
    ISqlBrowserResultProvider sqlBrowserResultProvider) : EditPageBase(binder)
{
    private const string QUERY_FIELDNAME = "QueryText";


    public override async Task<EditTemplateClientProperties> ConfigureTemplateProperties(EditTemplateClientProperties properties)
    {
        properties.SubmitButton.Label = "Run";
        properties.Items = await GetFormItemsClientProperties() as ICollection<IFormItemClientProperties>;

        return properties;
    }


    protected override Task<ICollection<IFormItem>> GetFormItems()
    {
        var formInfo = GetFormInfo();
        var elements = formInfo?.GetFormElements(true, false) ?? new List<IDataDefinitionItem>();
        var formItems = GetFormComponents(elements.OfType<FormFieldInfo>()).ToList<IFormItem>();

        return Task.FromResult(formItems as ICollection<IFormItem>);
    }


    protected override async Task<IEnumerable<IFormItemClientProperties>> GetFormItemsClientProperties()
    {
        var items = await GetFormItems();
        var components = items.OfType<IFormComponent>().ToList();
        await BindContextToComponents(components);

        return await items.OnlyVisible().GetClientProperties();
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


    private static FormInfo GetFormInfo()
    {
        var formInfo = new FormInfo();
        formInfo.AddFormItem(new FormFieldInfo
        {
            Name = QUERY_FIELDNAME,
            AllowEmpty = false,
            Visible = true,
            Precision = DataTypeManager.GetDataType(TypeEnum.Field, FieldDataType.LongText).DefaultPrecision,
            DataType = FieldDataType.LongText,
            Enabled = true,
            Settings = new() {
                {"controlname", TextAreaComponent.IDENTIFIER},
                {"CopyButtonVisible", false},
                {"MaxRowsNumber", 40},
                {"MinRowsNumber", 10}
            }
        });

        return formInfo;
    }
}
