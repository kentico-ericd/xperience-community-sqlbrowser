using Microsoft.SqlServer.TransactSql.ScriptDom;

using XperienceCommunity.SqlBrowser.Models;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Default implementation of <see cref="ISqlQueryValidator"/>.
/// </summary>
public class SqlQueryValidator(SqlBrowserOptions options) : ISqlQueryValidator
{
    private readonly TSql150Parser parser = new(true);


    public ValidationResult ValidateSqlStatement(string sqlStatement)
    {
        if (!options.UseSafeQuerySelect)
        {
            return ValidResult();
        }

        if (string.IsNullOrWhiteSpace(sqlStatement))
        {
            return InvalidResult("SQL statement cannot be empty or null.");
        }

        try
        {
            using var reader = new StringReader(sqlStatement);
            var fragment = parser.Parse(reader, out var errors);

            // Check for parsing errors
            if (errors.Any())
            {
                return InvalidResult($"SQL parsing error: {string.Join("; ", errors.Select(e => e.Message))}");
            }

            // Validate the statements
            var visitor = new SqlStatementVisitor();
            fragment.Accept(visitor);

            if (!visitor.IsValid)
            {
                return InvalidResult(visitor.ErrorMessage);
            }

            return ValidResult();
        }
        catch (Exception ex)
        {
            return InvalidResult($"Validation error: {ex.Message}");
        }
    }


    private static ValidationResult ValidResult() =>
        new()
        {
            IsValid = true,
            ErrorMessage = null
        };


    private static ValidationResult InvalidResult(string message) =>
        new()
        {
            IsValid = false,
            ErrorMessage = message
        };
}


internal class SqlStatementVisitor : TSqlFragmentVisitor
{
    public bool IsValid { get; private set; } = true;
    public string ErrorMessage { get; private set; } = string.Empty;
    private readonly HashSet<string> dangerousFunctions =
    [
        "OPENROWSET", "OPENDATASOURCE", "OPENQUERY", "OPENXML",
        "XP_CMDSHELL", "SP_CONFIGURE", "BULK"
    ];
    private readonly HashSet<Type> allowedStatementTypes =
    [
        typeof(SelectStatement),
        typeof(DeclareVariableStatement)
    ];


    public override void Visit(TSqlStatement node)
    {
        // Check if the statement type is allowed
        if (!allowedStatementTypes.Contains(node.GetType()))
        {
            IsValid = false;
            ErrorMessage = $"Statement type '{node.GetType().Name}' is not allowed. Only SELECT and DECLARE statements are permitted.";

            return;
        }

        base.Visit(node);
    }


    public override void Visit(TSqlFragment fragment)
    {
        // Workaround for parser missing OpenDataSourceTableReference
        if (fragment.ScriptTokenStream.Any(token => !string.IsNullOrEmpty(token.Text) &&
            token.Text.Equals("OPENDATASOURCE", StringComparison.OrdinalIgnoreCase)))
        {
            IsValid = false;
            ErrorMessage = "Function 'OPENDATASOURCE' is not allowed as it could be used for unauthorized operations.";
        }
    }


    public override void Visit(FunctionCall node)
    {
        // Check for potentially dangerous functions in SELECT statements
        string? functionName = node.FunctionName?.Value?.ToUpperInvariant();
        if (functionName is not null && dangerousFunctions.Contains(functionName))
        {
            IsValid = false;
            ErrorMessage = $"Function '{functionName}' is not allowed as it could be used for unauthorized operations.";

            return;
        }

        base.Visit(node);
    }


    public override void Visit(InsertStatement node)
    {
        IsValid = false;
        ErrorMessage = "INSERT statements are not allowed.";
    }


    public override void Visit(UpdateStatement node)
    {
        IsValid = false;
        ErrorMessage = "UPDATE statements are not allowed.";
    }


    public override void Visit(DeleteStatement node)
    {
        IsValid = false;
        ErrorMessage = "DELETE statements are not allowed.";
    }


    public override void Visit(CreateTableStatement node)
    {
        IsValid = false;
        ErrorMessage = "CREATE TABLE statements are not allowed.";
    }


    public override void Visit(DropTableStatement node)
    {
        IsValid = false;
        ErrorMessage = "DROP TABLE statements are not allowed.";
    }


    public override void Visit(AlterTableStatement node)
    {
        IsValid = false;
        ErrorMessage = "ALTER TABLE statements are not allowed.";
    }


    public override void Visit(TruncateTableStatement node)
    {
        IsValid = false;
        ErrorMessage = "TRUNCATE TABLE statements are not allowed.";
    }


    public override void Visit(CreateProcedureStatement node)
    {
        IsValid = false;
        ErrorMessage = "CREATE PROCEDURE statements are not allowed.";
    }


    public override void Visit(ExecuteStatement node)
    {
        IsValid = false;
        ErrorMessage = "EXECUTE statements are not allowed.";
    }


    public override void Visit(OpenRowsetTableReference node)
    {
        IsValid = false;
        ErrorMessage = "OPENROWSET function is not allowed as it could be used for unauthorized operations.";
    }

    public override void Visit(OpenQueryTableReference node)
    {
        IsValid = false;
        ErrorMessage = "OPENQUERY function is not allowed as it could be used for unauthorized operations.";
    }

    public override void Visit(OpenXmlTableReference node)
    {
        IsValid = false;
        ErrorMessage = "OPENXML function is not allowed as it could be used for unauthorized operations.";
    }
}
