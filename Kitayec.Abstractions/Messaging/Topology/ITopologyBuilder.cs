using Kitayec.Types;

namespace Kitayec.Abstractions.Messaging.Topology;

public interface ITopologyBuilder
{
    Task CreateTopologyAsync(
        string publisherSource,
        string consumerDestination,
        TopologyType topologyType,
        string filter = "",
        IDictionary<string, object>? consumerCustomArgs = default,
        CancellationToken cancellationToken = default);
}