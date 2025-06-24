using CMS.Helpers;
using CMS.Membership;

using System.Text.RegularExpressions;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;

using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Edit UI page which displays a database record in a dialog window.
/// </summary>
[UINavigation(false)]
[UIBreadcrumbs(false)]
[UIPageLocation(PageLocationEnum.Dialog)]
[UIEvaluatePermission(SystemPermissions.VIEW)]
public class ViewRecord(IFormDataBinder formDataBinder, ISqlBrowserResultProvider sqlBrowserResultProvider) : EditPageBase(formDataBinder)
{
    private const string EOL_REPLACEMENT = "#EOL#";
    private readonly Regex newLineRegex = RegexHelper.GetRegex(@"(<br[ ]?/>)|([\r]?\n)");


    /// <summary>
    /// Identifier of the result record to view.
    /// </summary>
    [PageParameter(typeof(IntPageModelBinder))]
    public int RecordId { get; set; }


    public override async Task ConfigurePage()
    {
        PageConfiguration.EditMode = FormEditMode.Disabled;
        PageConfiguration.SubmitConfiguration.Visible = false;

        await base.ConfigurePage();
    }


    public override async Task<EditTemplateClientProperties> ConfigureTemplateProperties(EditTemplateClientProperties properties)
    {
        string text = await sqlBrowserResultProvider.GetRowAsText(RecordId);
        text = newLineRegex.Replace(text, EOL_REPLACEMENT);
        properties.Items = [
            new TextWithLabelClientProperties()
            {
                ValueAsHtml = true,
                EditMode = FormEditMode.Disabled,
                ComponentName = "@kentico/xperience-admin-base/TextWithLabel",
                Value = HTMLHelper.HTMLEncode(text).Replace(EOL_REPLACEMENT, "<br />"),
            }
        ];

        return properties;
    }


    protected override Task<ICollection<IFormItem>> GetFormItems() =>
        Task.FromResult(Array.Empty<IFormItem>() as ICollection<IFormItem>);


    protected override Task<IEnumerable<IFormItemClientProperties>> GetFormItemsClientProperties() =>
        Task.FromResult(Enumerable.Empty<IFormItemClientProperties>());


    protected override Task<ICommandResponse> SubmitInternal(
        FormSubmissionCommandArguments args,
        ICollection<IFormItem> items,
        IFormFieldValueProvider formFieldValueProvider) => throw new NotImplementedException();
}
