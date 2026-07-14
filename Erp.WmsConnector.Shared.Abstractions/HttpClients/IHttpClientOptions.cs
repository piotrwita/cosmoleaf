using Erp.WmsConnector.Shared.Abstractions.Options;

namespace Erp.WmsConnector.Shared.Abstractions.HttpClients;

public interface IHttpClientOptions : IOptions
{
    string BaseUrl { get; set; }
    TimeSpan Timeout { get; set; }
}