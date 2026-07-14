using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Infrastructure.Authorization;

public sealed class AuthOptions : IOptions
{
    public static string SectionName => "Auth";

    public const string Scheme = "Bearer";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public TimeSpan? Expiry { get; init; }
}