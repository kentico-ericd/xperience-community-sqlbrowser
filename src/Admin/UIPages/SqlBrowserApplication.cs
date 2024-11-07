using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;

using XperienceCommunity.SqlBrowser.Admin.UIPages;

// Main application
[assembly: UIApplication(
    identifier: SqlBrowserApplicationPage.IDENTIFIER,
    type: typeof(SqlBrowserApplicationPage),
    slug: "sqlbrowser",
    name: "SQL browser",
    category: BaseApplicationCategories.DEVELOPMENT,
    icon: Icons.Database,
    templateName: TemplateNames.SECTION_LAYOUT)]

// Edit page
[assembly: UIPage(
    parentType: typeof(SqlBrowserApplicationPage),
    slug: "new",
    uiPageType: typeof(EditQuery),
    name: "New query",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.First)]

// Result page
[assembly: UIPage(
    parentType: typeof(SqlBrowserApplicationPage),
    slug: "results",
    uiPageType: typeof(ResultListing),
    name: "Results",
    templateName: TemplateNames.LISTING,
    order: UIPageOrder.NoOrder)]

// View record
[assembly: UIPage(
    typeof(ResultListing),
    PageParameterConstants.PARAMETERIZED_SLUG,
    typeof(ViewRecord),
    "View record",
    TemplateNames.EDIT,
    UIPageOrder.NoOrder)]


namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// The root application page for the SQL browser.
/// </summary>
public class SqlBrowserApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "XperienceCommunity.SqlBrowser";
}
