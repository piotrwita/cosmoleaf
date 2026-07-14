using System.Reflection;
using Erp.WmsConnector.Shared.Abstractions.Dispatching;
using Erp.WmsConnector.Shared.Abstractions.Dispatching.Commands;
using Erp.WmsConnector.Shared.Abstractions.Dispatching.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Erp.WmsConnector.Shared.Infrastructure.Dispatching;

public static class RegistrationExtensions
{
    /// <summary>
    /// Registers CQRS components (dispatchers, handlers, validators) with automatic discovery
    /// </summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services, IReadOnlyCollection<Assembly>? assemblies = null)
    {
        assemblies ??= AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToArray();

        services.TryAddTransient<IDispatcher, Dispatcher>();

        RegisterCommandHandlers(services, assemblies);
        RegisterQueryHandlers(services, assemblies);

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services, IReadOnlyCollection<Assembly> assemblies)
    {
        var commandHandlerTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetInterfaces().Any(IsCommandHandlerInterface))
            .Where(type => !IsDecoratorClass(type))
            .ToList();

        foreach (var handlerType in commandHandlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(IsCommandHandlerInterface)
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                services.TryAddTransient(interfaceType, handlerType);
            }
        }
    }

    private static void RegisterQueryHandlers(IServiceCollection services, IReadOnlyCollection<Assembly> assemblies)
    {
        var queryHandlerTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetInterfaces().Any(IsQueryHandlerInterface))
            .Where(type => !IsDecoratorClass(type))
            .ToList();

        foreach (var handlerType in queryHandlerTypes)
        {
            List<Type> interfaces = handlerType.GetInterfaces()
                .Where(IsQueryHandlerInterface)
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                services.TryAddTransient(interfaceType, handlerType);
            }
        }
    }

    private static bool IsCommandHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        Type genericDefinition = type.GetGenericTypeDefinition();

        return genericDefinition == typeof(ICommandHandler<>);
    }

    private static bool IsQueryHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        Type genericDefinition = type.GetGenericTypeDefinition();

        return genericDefinition == typeof(IQueryHandler<,>);
    }

    private static bool IsDecoratorClass(Type type)
    {
        // A decorator class is one that:
        // 1. Implements ICommandHandler<> or IQueryHandler<,>
        // 2. Has a constructor parameter of the same interface type it implements
        // 3. Has "Decorator" in its name (naming convention)

        if (!type.Name.Contains("Decorator", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var implementedInterfaces = type.GetInterfaces()
            .Where(i => IsCommandHandlerInterface(i) || IsQueryHandlerInterface(i))
            .ToList();

        if (!implementedInterfaces.Any())
        {
            return false;
        }

        var constructors = type.GetConstructors();
        if (constructors.Length != 1)
        {
            return false;
        }

        var constructor = constructors[0];
        var parameters = constructor.GetParameters();

        // Check if any constructor parameter matches one of the implemented interfaces
        return implementedInterfaces.Any(implementedInterface =>
            parameters.Any(param => param.ParameterType == implementedInterface));
    }
}
