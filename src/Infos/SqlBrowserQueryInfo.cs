using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using XperienceCommunity.SqlBrowser.Infos;

[assembly: RegisterObjectType(typeof(SqlBrowserQueryInfo), SqlBrowserQueryInfo.OBJECT_TYPE)]
namespace XperienceCommunity.SqlBrowser.Infos;

/// <summary>
/// Data container class for <see cref="SqlBrowserQueryInfo"/>.
/// </summary>
[Serializable]
public partial class SqlBrowserQueryInfo : AbstractInfo<SqlBrowserQueryInfo>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "xperiencecommunity.sqlbrowserquery";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(
        typeof(SqlBrowserQueryInfo),
        OBJECT_TYPE,
        "XperienceCommunity.SqlBrowserQuery",
        nameof(SqlBrowserQueryId),
        null, null, null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        ContinuousIntegrationSettings =
        {
            Enabled = true,
        },
    };


    /// <summary>
    /// Query id.
    /// </summary>
    [DatabaseField]
    public virtual int SqlBrowserQueryId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(SqlBrowserQueryId)), 0);
        set => SetValue(nameof(SqlBrowserQueryId), value);
    }


    /// <summary>
    /// Query text.
    /// </summary>
    [DatabaseField]
    public virtual string SqlBrowserQueryText
    {
        get => ValidationHelper.GetString(GetValue(nameof(SqlBrowserQueryText)), string.Empty);
        set => SetValue(nameof(SqlBrowserQueryText), value);
    }


    /// <summary>
    /// Constructor for de-serialization.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
    protected SqlBrowserQueryInfo(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }


    /// <summary>
    /// Creates an empty instance of the <see cref="SqlBrowserQueryInfo"/> class.
    /// </summary>
    public SqlBrowserQueryInfo()
        : base(TYPEINFO)
    {
    }


    /// <summary>
    /// Creates a new instances of the <see cref="SqlBrowserQueryInfo"/> class from the given <see cref="DataRow"/>.
    /// </summary>
    /// <param name="dr">DataRow with the object data.</param>
    public SqlBrowserQueryInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
