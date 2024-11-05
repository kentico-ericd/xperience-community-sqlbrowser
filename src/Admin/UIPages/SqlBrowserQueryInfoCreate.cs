using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;

using XperienceCommunity.SqlBrowser.Infos;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;
[UINavigation(false)]
public class SqlBrowserQueryInfoCreate : CreatePage<SqlBrowserQueryInfo, ResultListing>
{
    private readonly ISqlBrowserResultProvider sqlBrowserQueryProvider;


    public SqlBrowserQueryInfoCreate(
        IFormComponentMapper mapper,
        IFormDataBinder binder,
        IPageUrlGenerator generator,
        ISqlBrowserResultProvider sqlBrowserQueryProvider)
        : base(mapper, binder, generator) => this.sqlBrowserQueryProvider = sqlBrowserQueryProvider;


    public override Task ConfigurePage()
    {
        PageConfiguration.SubmitConfiguration.Label = "Run";

        return base.ConfigurePage();
    }


    public override Task<ICommandResponse> Submit(FormSubmissionCommandArguments args)
    {
        string queryText = args.Data[nameof(SqlBrowserQueryInfo.SqlBrowserQueryText)].GetString()
            ?? throw new NullReferenceException(nameof(SqlBrowserQueryInfo.SqlBrowserQueryText));

        sqlBrowserQueryProvider.SetQuery(new SqlBrowserQueryInfo
        {
            SqlBrowserQueryText = queryText
        });

        string navigationUrl = pageUrlGenerator.GenerateUrl<ResultListing>();

        return Task.FromResult((ICommandResponse)NavigateTo(navigationUrl));
    }
}
