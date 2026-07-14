using System.Data;
using System.Text.RegularExpressions;
using Erp.WmsConnector.Shared.Abstractions.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Erp.WmsConnector.Shared.Infrastructure.Database;

/// <summary>
/// Extension methods for executing stored procedures with structured parameters.
/// </summary>
public static partial class StoredProcedureExtensions
{
    [GeneratedRegex(@"^@?[\w\.\[\]]+$", RegexOptions.Compiled)]
    private static partial Regex SqlIdentifierRegex();

    /// <summary>
    /// Executes a stored procedure with a table-valued parameter and returns the deserialized JSON result.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the JSON result into.</typeparam>
    /// <param name="database">The database facade.</param>
    /// <param name="procedureName">The name of the stored procedure (e.g., "Schema.ProcedureName").</param>
    /// <param name="inputTable">The DataTable containing the input data.</param>
    /// <param name="inputParameterName">The name of the input parameter (e.g., "@inputArticles").</param>
    /// <param name="tableTypeName">The SQL Server table type name (e.g., "Schema.TableTypeName").</param>
    /// <param name="serializer">The serializer to deserialize the JSON result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized result, or default if the result is empty.</returns>
    /// <exception cref="InvalidDataException">Thrown when the stored procedure returns no result.</exception>
    public static async Task<TResult?> ExecuteStructuredProcedureAsync<TResult>(
        this DatabaseFacade database,
        string procedureName,
        DataTable inputTable,
        string inputParameterName,
        string tableTypeName,
        ISerializer serializer,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        return await ExecuteStructuredProcedureCoreAsync<TResult>(
            procedureName,
            inputTable,
            inputParameterName,
            tableTypeName,
            serializer,
            database.ExecuteSqlRawAsync,
            cancellationToken);
    }

    /// <summary>
    /// Executes a stored procedure with a table-valued parameter and returns the deserialized JSON result,
    /// or a default value if the result is null.
    /// </summary>
    /// <typeparam name="TResult">The type to deserialize the JSON result into.</typeparam>
    /// <param name="database">The database facade.</param>
    /// <param name="procedureName">The name of the stored procedure (e.g., "Schema.ProcedureName").</param>
    /// <param name="inputTable">The DataTable containing the input data.</param>
    /// <param name="inputParameterName">The name of the input parameter (e.g., "@inputArticles").</param>
    /// <param name="tableTypeName">The SQL Server table type name (e.g., "Schema.TableTypeName").</param>
    /// <param name="serializer">The serializer to deserialize the JSON result.</param>
    /// <param name="defaultValue">The default value to return if deserialization returns null.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized result, or the default value if the result is null.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("csharpsquid", "S107", Justification = "All parameters are required for stored procedure execution. Reducing parameters would require creating a parameter object, which adds unnecessary complexity for this extension method.")]
    public static async Task<TResult> ExecuteStructuredProcedureAsync<TResult>(
        this DatabaseFacade database,
        string procedureName,
        DataTable inputTable,
        string inputParameterName,
        string tableTypeName,
        ISerializer serializer,
        TResult defaultValue,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        TResult? result = await ExecuteStructuredProcedureCoreAsync<TResult>(
            procedureName,
            inputTable,
            inputParameterName,
            tableTypeName,
            serializer,
            database.ExecuteSqlRawAsync,
            cancellationToken);

        return result ?? defaultValue;
    }

    /// <summary>
    /// Core implementation for executing structured procedures. This method is internal to allow unit testing
    /// by injecting a mock database executor delegate.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("csharpsquid", "S2077", Justification = "SQL identifiers are validated via ValidateSqlIdentifier to prevent SQL injection. These cannot be parameterized as they are identifiers, not values.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("csharpsquid", "S107", Justification = "All parameters are required for stored procedure execution. Reducing parameters would require creating a parameter object, which adds unnecessary complexity for this extension method.")]
    internal static async Task<TResult?> ExecuteStructuredProcedureCoreAsync<TResult>(
        string procedureName,
        DataTable inputTable,
        string inputParameterName,
        string tableTypeName,
        ISerializer serializer,
        Func<string, SqlParameter[], CancellationToken, Task<int>> executeSqlAsync,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        ValidateSqlIdentifier(procedureName, nameof(procedureName));
        ValidateSqlIdentifier(inputParameterName, nameof(inputParameterName));

        SqlParameter inputParameter = new(inputParameterName, SqlDbType.Structured)
        {
            Value = inputTable,
            TypeName = tableTypeName
        };

        SqlParameter resultParameter = new("@result", SqlDbType.NVarChar, -1)
        {
            Direction = ParameterDirection.Output
        };

        string sql = $"EXEC {procedureName} {inputParameterName}, @result OUTPUT";
        await executeSqlAsync(sql, [inputParameter, resultParameter], cancellationToken);

        string? jsonResult = resultParameter.Value?.ToString();
        if (string.IsNullOrEmpty(jsonResult))
        {
            throw new InvalidDataException($"The stored procedure '{procedureName}' did not return any result");
        }

        return serializer.Deserialize<TResult>(jsonResult);
    }

    private static void ValidateSqlIdentifier(string value, string parameterName)
    {
        if (!SqlIdentifierRegex().IsMatch(value))
        {
            throw new ArgumentException($"Invalid SQL identifier: '{value}'", parameterName);
        }
    }
}
