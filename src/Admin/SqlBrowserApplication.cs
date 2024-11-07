using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;

using XperienceCommunity.SqlBrowser.Admin;
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
    parentType: typeof(ResultListing),
    slug: PageParameterConstants.PARAMETERIZED_SLUG,
    uiPageType: typeof(ViewRecord),
    name: "View record",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.NoOrder)]


namespace XperienceCommunity.SqlBrowser.Admin;

/// <summary>
/// The root application page for the SQL browser.
/// </summary>
public class SqlBrowserApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "XperienceCommunity.SqlBrowser";
}
