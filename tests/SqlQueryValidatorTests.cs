using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Tests;

public class SqlQueryValidatorTests
{
    private static readonly string[] unsafeStatements = [
        "INSERT INTO CMS_Country VALUES ('MyCountry', 'My country', NEWID(), GETDATE(), 'my', 'myc')",
        "UPDATE CMS_User SET UserEnabled = 0",
        "DELETE FROM CMS_User",
        "CREATE TABLE test (test_id int)",
        "DROP TABLE CMS_User",
        "ALTER TABLE CMS_User DROP COLUMN UserEnabled",
        "TRUNCATE TABLE CMS_User",
        "CREATE PROCEDURE test AS SELECT UserID FROM CMS_User",
        "CREATE PROC test AS SELECT UserID FROM CMS_User",
        "EXECUTE ('SELECT * FROM CMS_User')",
        "EXEC ('SELECT * FROM CMS_User')",
        "SELECT 1;INSERT INTO CMS_Country VALUES ('MyCountry', 'My country', NEWID(), GETDATE(), 'my', 'myc')",
        "SELECT * FROM OPENROWSET('SQLNCLI', 'Server=.;Trusted_Connection=yes;', 'SELECT * FROM sys.tables')",
        "SELECT * FROM OPENDATASOURCE('MSOLEDBSQL', 'Server=myserver;Database=mydb;TrustServerCertificate=Yes;Trusted_Connection=Yes;').MyTable",
        "SELECT * FROM OPENQUERY(MyServer, 'DELETE FROM CMS_User')",
        @"SELECT * FROM OPENXML(@xml, '/ROOT/Customer/Order/OrderDetail', 2) WITH (
            CustomerID VARCHAR(10) '../@CustomerID'
        )",
        "EXECUTE xp_cmdshell 'dir *.exe'",
        "EXECUTE sp_configure 'xp_cmdshell', 1",
        "BULK INSERT MyForm FROM 'https://test.blob.core.windows.net/public/hax/*.csv'"
    ];
    private static readonly string[] safeStatements = [
        "SELECT * FROM CMS_User",
        "DECLARE @Enabled bit = 1;SELECT * FROM CMS_User WHERE UserEnabled = @Enabled",
        "SELECT * FROM CMS_EventLog WHERE EventCode LIKE '%create%'",
         @"-- This is a comment
              DECLARE @UserId int = 123;
              SELECT UserName, Email 
              FROM CMS_User 
              WHERE UserID = @UserId",
    ];


    [TestCaseSource(nameof(unsafeStatements))]
    public void SafeSelectEnabled_UnsafeStatement_IsNotValid(string query)
    {
        var validator = GetValidator(true);
        var result = validator.ValidateSqlStatement(query);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            // Ensure error message is from visit and not generic error
            Assert.That(result.ErrorMessage, Does.Not.Contain("Validation error"));
            Assert.That(result.ErrorMessage, Does.Not.Contain("SQL parsing error"));
            Assert.That(result.ErrorMessage, Is.Not.EqualTo("SQL statement cannot be empty or null."));
        });
    }


    [TestCaseSource(nameof(unsafeStatements))]
    public void SafeSelectDisabled_UnsafeStatement_IsValid(string query)
    {
        var validator = GetValidator(false);
        var result = validator.ValidateSqlStatement(query);

        Assert.That(result.IsValid, Is.True);
    }


    [TestCaseSource(nameof(safeStatements))]
    public void SafeStatement_IsValid(string query)
    {
        var validator = GetValidator(true);
        var result = validator.ValidateSqlStatement(query);

        Assert.That(result.IsValid, Is.True);
    }


    private static SqlQueryValidator GetValidator(bool safeSelectEnabled) =>
        new(new() { UseSafeQuerySelect = safeSelectEnabled });
}
