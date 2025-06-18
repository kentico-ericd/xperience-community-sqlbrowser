using Microsoft.SqlServer.TransactSql.ScriptDom;

using XperienceCommunity.SqlBrowser.Models;

namespace XperienceCommunity.SqlBrowser.Helpers;
public class SqlStatementValidator
{
    private readonly TSql150Parser parser;

    public SqlStatementValidator() => parser = new TSql150Parser(true); // true for quoted identifiers

    public ValidationResult ValidateSqlStatement(string sqlStatement)
    {
        if (string.IsNullOrWhiteSpace(sqlStatement))
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "SQL statement cannot be empty or null."
            };
        }

        try
        {
            using var reader = new StringReader(sqlStatement);
            var fragment = parser.Parse(reader, out var errors);

            // Check for parsing errors
            if (errors.Any())
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"SQL parsing error: {string.Join("; ", errors.Select(e => e.Message))}"
                };
            }

            // Validate the statements
            var visitor = new SqlStatementVisitor();
            fragment.Accept(visitor);

            if (!visitor.IsValid)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = visitor.ErrorMessage
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Validation error: {ex.Message}"
            };
        }
    }
}

public class SqlStatementVisitor : TSqlFragmentVisitor
{
    public bool IsValid { get; private set; } = true;
    public string ErrorMessage { get; private set; } = string.Empty;

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

    // Explicitly block dangerous statements
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

    // Allow comments - they don't generate visit calls, so no action needed

    // Check for potentially dangerous functions in SELECT statements
    public override void Visit(FunctionCall node)
    {
        string? functionName = node.FunctionName?.Value?.ToUpperInvariant();

        var dangerousFunctions = new HashSet<string>
        {
            "OPENROWSET", "OPENDATASOURCE", "OPENQUERY", "OPENXML",
            "XP_CMDSHELL", "SP_CONFIGURE", "BULK"
        };

        if (functionName != null && dangerousFunctions.Contains(functionName))
        {
            IsValid = false;
            ErrorMessage = $"Function '{functionName}' is not allowed as it could be used for unauthorized operations.";
            return;
        }

        base.Visit(node);
    }
}
