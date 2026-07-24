namespace Kitayec.Abstractions.Messaging.Topology;

public interface ITopologyConventionProvider
{
    string ResolveExchangeName(string name);
    string ResolveQueueName(string name);
}