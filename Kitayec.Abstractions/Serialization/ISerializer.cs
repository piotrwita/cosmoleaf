using System.Text.Json;

namespace Kitayec.Abstractions.Serialization;

public interface ISerializer
{
    string Serialize(object value, JsonSerializerOptions? options = null);
    TObject? Deserialize<TObject>(string json, JsonSerializerOptions? options = null);
    object Deserialize(string json, Type type, JsonSerializerOptions? options = null);
    byte[] SerializeBinary(object value, JsonSerializerOptions? options = null);
    TObject? DeserializeBinary<TObject>(byte[] data, JsonSerializerOptions? options = null);
}