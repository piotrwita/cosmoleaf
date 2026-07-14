using System.Collections.Concurrent;
using System.Reflection;
using Erp.WmsConnector.Shared.Abstractions.Dispatching;
using Erp.WmsConnector.Shared.Abstractions.Dispatching.Commands;
using Erp.WmsConnector.Shared.Abstractions.Dispatching.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.WmsConnector.Shared.Infrastructure.Dispatching;

/// <summary>
/// Unified dispatcher implementation for commands and queries (CQRS pattern)
/// </summary>
internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    /// <summary>
    /// Cache for reflection-based method lookups to avoid repeated reflection overhead
    /// </summary>
    private static readonly ConcurrentDictionary<(Type QueryType, Type ResultType), MethodInfo> MethodCache = new();

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        Type queryType = query.GetType();
        Type resultType = typeof(TResult);
        var cacheKey = (queryType, resultType);

        MethodInfo method = MethodCache.GetOrAdd(cacheKey, static key =>
        {
            var targetMethod = typeof(Dispatcher).GetMethods()
                .First(m => m.Name == nameof(Query) && m.IsGenericMethodDefinition);

            return targetMethod.MakeGenericMethod(key.QueryType, key.ResultType);
        });

        try
        {
            var task = (Task<TResult>)method.Invoke(this, [query, cancellationToken])!;
            return await task;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            // Unwrap the original exception to preserve the exception type
            throw ex.InnerException;
        }
    }

    public async Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(query);

        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result;
    }
}