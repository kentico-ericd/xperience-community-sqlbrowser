using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;

using XperienceCommunity.SqlBrowser.Admin.Generated;

namespace XperienceCommunity.SqlBrowser.Admin;

/// <summary>
/// Initializes database objects for the SQL browser module at application startup.
/// </summary>
public class SqlBrowserInstaller(IInfoProvider<ResourceInfo> resourceProvider)
{
    private const string RESOURCE_NAME = "Community.SqlBrowser";


    public void Install()
    {
        var resource = resourceProvider.Get(RESOURCE_NAME) ?? new ResourceInfo();

        InitializeResource(resource);
        InstallSavedQueryInfo(resource);
    }


    public ResourceInfo InitializeResource(ResourceInfo resource)
    {
        resource.ResourceDisplayName = "Xperience Community- SQL browser";
        resource.ResourceName = RESOURCE_NAME;
        resource.ResourceDescription = "SQL browser module";
        resource.ResourceIsInDevelopment = false;
        if (resource.HasChanged)
        {
            resourceProvider.Set(resource);
        }

        return resource;
    }


    public static void InstallSavedQueryInfo(ResourceInfo resource)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(SqlBrowserSavedQueryInfo.OBJECT_TYPE) ??
            DataClassInfo.New(SqlBrowserSavedQueryInfo.OBJECT_TYPE);

        info.ClassName = SqlBrowserSavedQueryInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = SqlBrowserSavedQueryInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "SQL browser saved query";
        info.ClassType = ClassType.OTHER;
        info.ClassResourceID = resource.ResourceID;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryId));
        var formItem = new FormFieldInfo
        {
            Name = nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryGuid),
            AllowEmpty = false,
            Visible = true,
            Precision = 0,
            DataType = "guid",
            Enabled = true,
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryName),
            AllowEmpty = false,
            Visible = true,
            Precision = 0,
            Size = 100,
            DataType = "text",
            Enabled = true
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryText),
            AllowEmpty = false,
            Visible = true,
            Precision = 0,
            Size = 4000,
            DataType = "text",
            Enabled = true
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryOrder),
            AllowEmpty = false,
            Visible = true,
            Precision = 0,
            DataType = "integer",
            Enabled = true
        };
        formInfo.AddFormItem(formItem);

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }


    /// <summary>
    /// Ensure that the form is upserted with any existing form.
    /// </summary>
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
