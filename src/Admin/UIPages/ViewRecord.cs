using System.Text.RegularExpressions;

using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.Base.Forms.Internal;

using XperienceCommunity.SqlBrowser.Services;

using static XperienceCommunity.SqlBrowser.Admin.UIPages.ViewRecord;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Edit UI page which displays a database record in a dialog window.
/// </summary>
[UINavigation(false)]
[UIBreadcrumbs(false)]
[UIPageLocation(PageLocationEnum.Dialog)]
[UIEvaluatePermission(SystemPermissions.VIEW)]
public class ViewRecord(
    IFormDataBinder formDataBinder,
    ISqlBrowserResultProvider sqlBrowserResultProvider,
    IFormItemCollectionProvider formItemCollectionProvider) : ModelEditPage<SqlBrowserResultModel>(formItemCollectionProvider, formDataBinder)
{
    private SqlBrowserResultModel? model;
    private const string EOL_REPLACEMENT = "#EOL#";


    /// <summary>
    /// Identifier of the result record to view.
    /// </summary>
    [PageParameter(typeof(IntPageModelBinder))]
    public int RecordId { get; set; }


    protected override SqlBrowserResultModel Model => model ??= new();


    public override async Task ConfigurePage()
    {
        PageConfiguration.EditMode = FormEditMode.Disabled;
        PageConfiguration.SubmitConfiguration.Visible = false;

        string text = await sqlBrowserResultProvider.GetRowAsText(RecordId);
        var newLineRegex = RegexHelper.GetRegex(@"(<br[ ]?/>)|([\r]?\n)");
        text = newLineRegex.Replace(text, EOL_REPLACEMENT);
        Model.RowText = HTMLHelper.HTMLEncode(text).Replace(EOL_REPLACEMENT, "<br />");

        await base.ConfigurePage();
    }


    protected override async Task<ICollection<IFormItem>> GetFormItems()
    {
        var formItems = await base.GetFormItems();
        var rowTextItem = formItems
            .OfType<TextWithLabelComponent>()
            .FirstOrDefault(c => c.Name == nameof(SqlBrowserResultModel.RowText));
        rowTextItem?.Properties.ValueAsHtml = true;

        return formItems;
    }


    public class SqlBrowserResultModel
    {
        [TextWithLabelComponent]
        public string? RowText { get; set; }
    }
}
