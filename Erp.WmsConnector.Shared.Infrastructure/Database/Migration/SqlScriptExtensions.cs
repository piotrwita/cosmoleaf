using Microsoft.EntityFrameworkCore.Migrations;

namespace Erp.WmsConnector.Shared.Infrastructure.Database.Migration;

public static class SqlScriptExtensions
{
    public static MigrationBuilder CreateProcedure(this MigrationBuilder migrationBuilder, string procedureName)
        => migrationBuilder.ExecuteScript(procedureName, ScriptType.Procedures);

    public static MigrationBuilder CreateFunction(this MigrationBuilder migrationBuilder, string functionName)
        => migrationBuilder.ExecuteScript(functionName, ScriptType.Functions);

    public static MigrationBuilder CreateType(this MigrationBuilder migrationBuilder, string typeName)
        => migrationBuilder.ExecuteScript(typeName, ScriptType.Types);

    private static MigrationBuilder ExecuteScript(this MigrationBuilder migrationBuilder, string scriptName, ScriptType sType)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string scriptPath = Path.Combine(baseDirectory, "Data/EF/Migrations/Scripts", sType.ToString(), $"{scriptName}.sql");

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"SQL script not found: {scriptPath}");
        }

        string sql = File.ReadAllText(scriptPath);
        migrationBuilder.Sql(sql);

        return migrationBuilder;
    }

    public static MigrationBuilder DropProcedure(this MigrationBuilder migrationBuilder, string procedureName)
    {
        migrationBuilder.Sql($@"
            IF OBJECT_ID('{procedureName}', 'P') IS NOT NULL
                DROP PROCEDURE {procedureName}");

        return migrationBuilder;
    }

    public static MigrationBuilder DropFunction(this MigrationBuilder migrationBuilder, string functionName)
    {
        migrationBuilder.Sql($@"
            IF OBJECT_ID('{functionName}', 'FN') IS NOT NULL OR 
               OBJECT_ID('{functionName}', 'IF') IS NOT NULL OR 
               OBJECT_ID('{functionName}', 'TF') IS NOT NULL
                DROP FUNCTION {functionName}");

        return migrationBuilder;
    }

    public static MigrationBuilder DropType(this MigrationBuilder migrationBuilder, string typeName)
    {
        migrationBuilder.Sql($@"
            IF TYPE_ID('{typeName}') IS NOT NULL
                DROP TYPE {typeName}");

        return migrationBuilder;
    }
}