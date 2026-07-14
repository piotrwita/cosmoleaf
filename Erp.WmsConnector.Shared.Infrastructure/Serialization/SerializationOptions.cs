using System.Text.Json;
using System.Text.Json.Serialization;

namespace Erp.WmsConnector.Shared.Infrastructure.Serialization;

public static class SerializationOptions
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static readonly JsonSerializerOptions PrettyPrint = new(Default)
    {
        WriteIndented = true,
    };
}