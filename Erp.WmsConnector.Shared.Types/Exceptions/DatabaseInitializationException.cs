using Microsoft.AspNetCore.Http;

namespace Erp.WmsConnector.Shared.Types.Exceptions;

public class DatabaseInitializationException : BaseException
{
    private const string DefaultMessage = "An error occurred while trying to update the database. Please check your database connection and migration scripts.";
    public DatabaseInitializationException()
        : base(DefaultMessage)
    { }

    public DatabaseInitializationException(Exception innerException)
        : base(DefaultMessage, innerException)
    { }

    public override int StatusCode => StatusCodes.Status500InternalServerError;
}