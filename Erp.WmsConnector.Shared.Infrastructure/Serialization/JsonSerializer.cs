using System.Text.Json;
using Erp.WmsConnector.Shared.Abstractions.Serialization;
using SystemTextJson = System.Text.Json.JsonSerializer;

namespace Erp.WmsConnector.Shared.Infrastructure.Serialization;

internal sealed class JsonSerializer : ISerializer
{
    public string Serialize(object value, JsonSerializerOptions? options = null)
        => SystemTextJson.Serialize(value, value.GetType(), options ?? SerializationOptions.Default);

    public TObject? Deserialize<TObject>(string json, JsonSerializerOptions? options = null)
        => SystemTextJson.Deserialize<TObject>(json, options ?? SerializationOptions.Default);

    public object? Deserialize(string json, Type type, JsonSerializerOptions? options = null)
        => SystemTextJson.Deserialize(json, type, options ?? SerializationOptions.Default);

    public byte[] SerializeBinary(object value, JsonSerializerOptions? options = null)
        => SystemTextJson.SerializeToUtf8Bytes(value, value.GetType(), options ?? SerializationOptions.Default);

    public TObject? DeserializeBinary<TObject>(byte[] data, JsonSerializerOptions? options = null)
        => SystemTextJson.Deserialize<TObject>(data.AsSpan(), options ?? SerializationOptions.Default);
}