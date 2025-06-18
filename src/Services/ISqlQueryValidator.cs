using XperienceCommunity.SqlBrowser.Models;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Contains methods for validating SQL statements.
/// </summary>
public interface ISqlQueryValidator
{
    /// <summary>
    /// Validates the statement's syntax and security. By default, only SELECT and DECLARE statements are valid.
    /// </summary>
    public ValidationResult ValidateSqlStatement(string sqlStatement);
}
