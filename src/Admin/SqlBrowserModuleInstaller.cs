using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;

using XperienceCommunity.SqlBrowser.Admin.UIPages;
using XperienceCommunity.SqlBrowser.Infos;

namespace XperienceCommunity.SqlBrowser.Admin;
internal class SqlBrowserModuleInstaller
{
    private readonly IInfoProvider<ResourceInfo> resourceProvider;


    public SqlBrowserModuleInstaller(IInfoProvider<ResourceInfo> resourceProvider) => this.resourceProvider = resourceProvider;


    public void Install()
    {
        var resource = resourceProvider.Get(SqlBrowserApplicationPage.IDENTIFIER)
            ?? new ResourceInfo();

        InitializeResource(resource);
        var queryInfo = InstallSqlQueryInfo(resource);
        InstallSqlQueryInfoEditForm(queryInfo);
    }


    private void InitializeResource(ResourceInfo resource)
    {
        resource.ResourceDisplayName = "SQL browser";
        resource.ResourceName = SqlBrowserApplicationPage.IDENTIFIER;
        resource.ResourceIsInDevelopment = false;
        if (resource.HasChanged)
        {
            resourceProvider.Set(resource);
        }
    }


    private static DataClassInfo InstallSqlQueryInfo(ResourceInfo resource)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(SqlBrowserQueryInfo.OBJECT_TYPE) ?? DataClassInfo.New(SqlBrowserQueryInfo.OBJECT_TYPE);

        info.ClassName = SqlBrowserQueryInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = SqlBrowserQueryInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "SQL browser query";
        info.ClassType = ClassType.OTHER;
        info.ClassResourceID = resource.ResourceID;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(SqlBrowserQueryInfo.SqlBrowserQueryId));
        var formItem = new FormFieldInfo
        {
            Name = nameof(SqlBrowserQueryInfo.SqlBrowserQueryText),
            AllowEmpty = false,
            Visible = true,
            Precision = 200,
            DataType = "longtext",
            Enabled = true
        };
        formInfo.AddFormItem(formItem);
        SetFormDefinition(info, formInfo);

        if (info.ClassID == 0)
        {
            info.Insert();
        }
        else if (info.HasChanged)
        {
            info.Update();
        }

        return info;
    }


    private static void InstallSqlQueryInfoEditForm(DataClassInfo info)
    {
        var altForm = AlternativeFormInfoProvider.GetAlternativeForms()
            .WhereEquals(nameof(AlternativeFormInfo.FormClassID), info.ClassID)
            .WhereEquals(nameof(AlternativeFormInfo.FormName), "create")
            .FirstOrDefault()
            ?? new AlternativeFormInfo();
        var existingForm = new FormInfo(altForm.FormDefinition);

        altForm.FormName = "create";
        altForm.FormDisplayName = "Create";
        altForm.FormClassID = info.ClassID;

        var newForm = new FormInfo();
        var queryTextField = new FormFieldInfo()
        {
            Name = nameof(SqlBrowserQueryInfo.SqlBrowserQueryText),
            AllowEmpty = false,
            Visible = true,
            Precision = 200,
            DataType = "longtext",
            Enabled = true,
            Caption = "Query",
            Settings = new() {
                {"controlname", "Kentico.Administration.TextArea"},
                {"CopyButtonVisible", false},
                {"MaxRowsNumber", 40},
                {"MinRowsNumber", 10}
            }
        };
        newForm.AddFormItem(queryTextField);

        existingForm.CombineWithForm(newForm, new());
        altForm.FormDefinition = existingForm.GetXmlDefinition();
        if (altForm.HasChanged)
        {
            AlternativeFormInfoProvider.SetAlternativeFormInfo(altForm);
        }
    }


    private static void SetFormDefinition(DataClassInfo info, FormInfo form)
    {
        if (info.ClassID > 0)
        {
            var existingForm = new FormInfo(info.ClassFormDefinition);
            existingForm.CombineWithForm(form, new());
            info.ClassFormDefinition = existingForm.GetXmlDefinition();
        }
        else
        {
            info.ClassFormDefinition = form.GetXmlDefinition();
        }
    }
}
