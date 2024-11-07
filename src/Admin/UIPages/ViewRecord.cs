using CMS.Helpers;

using System.Text.RegularExpressions;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;

using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Edit UI page which displays a database record in a dialog window.
/// </summary>
[UIPageLocation(PageLocationEnum.Dialog)]
[UINavigation(false)]
[UIBreadcrumbs(false)]
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
        PageConfiguration.Disabled = true;
        PageConfiguration.SubmitConfiguration.Visible = false;

        await base.ConfigurePage();
    }

    public override Task<EditTemplateClientProperties> ConfigureTemplateProperties(EditTemplateClientProperties properties)
    {
        string text = sqlBrowserResultProvider.GetRowAsText(RecordId);
        text = newLineRegex.Replace(text, EOL_REPLACEMENT);
        properties.Items = [
            new TextWithLabelClientProperties()
            {
                Disabled = true,
                ValueAsHtml = true,
                ComponentName = "@kentico/xperience-admin-base/TextWithLabel",
                Value = HTMLHelper.HTMLEncode(text).Replace(EOL_REPLACEMENT, "<br />"),
            }
        ];

        return Task.FromResult(properties);
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
