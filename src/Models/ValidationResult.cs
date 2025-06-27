using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Models;

/// <summary>
/// The result of a <see cref="ISqlQueryValidator"/> validation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// <c>True</c> if the validation succeeded.
    /// </summary>
    public bool IsValid { get; set; }


    /// <summary>
    /// Contains a validation message if the validation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
