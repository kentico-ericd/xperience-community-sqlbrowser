using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using XperienceCommunity.SqlBrowser.Admin.Generated;

[assembly: RegisterObjectType(typeof(SqlBrowserSavedQueryInfo), SqlBrowserSavedQueryInfo.OBJECT_TYPE)]

namespace XperienceCommunity.SqlBrowser.Admin.Generated;

/// <summary>
/// Data container class for <see cref="SqlBrowserSavedQueryInfo"/>.
/// </summary>
[Serializable]
public partial class SqlBrowserSavedQueryInfo : AbstractInfo<SqlBrowserSavedQueryInfo, IInfoProvider<SqlBrowserSavedQueryInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "sqlbrowser.savedquery";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(
        typeof(IInfoProvider<SqlBrowserSavedQueryInfo>),
        OBJECT_TYPE,
        "SqlBrowser.SavedQuery",
        nameof(SqlBrowserSavedQueryId), null,
        nameof(SqlBrowserSavedQueryGuid),
        nameof(SqlBrowserSavedQueryName), null, null, null, null)
    {
        TouchCacheDependencies = true,
        ContinuousIntegrationSettings =
        {
            Enabled = true,
        },
    };


    /// <summary>
    /// Saved query id.
    /// </summary>
    [DatabaseField]
    public virtual int SqlBrowserSavedQueryId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(SqlBrowserSavedQueryId)), 0);
        set => SetValue(nameof(SqlBrowserSavedQueryId), value);
    }


    /// <summary>
    /// Saved query GUID.
    /// </summary>
    [DatabaseField]
    public virtual Guid SqlBrowserSavedQueryGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(SqlBrowserSavedQueryGuid)), default);
        set => SetValue(nameof(SqlBrowserSavedQueryGuid), value);
    }


    /// <summary>
    /// Saved query name.
    /// </summary>
    [DatabaseField]
    public virtual string SqlBrowserSavedQueryName
    {
        get => ValidationHelper.GetString(GetValue(nameof(SqlBrowserSavedQueryName)), string.Empty);
        set => SetValue(nameof(SqlBrowserSavedQueryName), value);
    }


    /// <summary>
    /// Saved query text.
    /// </summary>
    [DatabaseField]
    public virtual string SqlBrowserSavedQueryText
    {
        get => ValidationHelper.GetString(GetValue(nameof(SqlBrowserSavedQueryText)), string.Empty);
        set => SetValue(nameof(SqlBrowserSavedQueryText), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    /// <summary>
    /// Creates an empty instance of the <see cref="SqlBrowserSavedQueryInfo"/> class.
    /// </summary>
    public SqlBrowserSavedQueryInfo()
        : base(TYPEINFO)
    {
    }


    /// <summary>
    /// Creates a new instances of the <see cref="SqlBrowserSavedQueryInfo"/> class from the given <see cref="DataRow"/>.
    /// </summary>
    /// <param name="dr">DataRow with the object data.</param>
    public SqlBrowserSavedQueryInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
