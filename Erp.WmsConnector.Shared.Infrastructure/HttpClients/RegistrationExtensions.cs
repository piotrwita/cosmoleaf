using System.Diagnostics.CodeAnalysis;
using Erp.WmsConnector.Shared.Abstractions.HttpClients;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;

namespace Erp.WmsConnector.Shared.Infrastructure.HttpClients;

public static class RegistrationExtensions
{
    /// <summary>
    /// Registers a typed HttpClient with basic configuration
    /// </summary>
    public static IHttpClientBuilder AddTypedHttpClient<TClient>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttpClient>? configureClient = null)
        where TClient : class
    {
        var httpClientOptions = configuration.GetRequiredOptions<HttpClientOptions>();

        return services.AddHttpClient<TClient>((serviceProvider, client) =>
        {
            client.AddDefaultRequestHeaders();

            configureClient?.Invoke(client);
        })
        .AddStandardPolicies(httpClientOptions);
    }

    /// <summary>
    /// Registers a typed HttpClient with options configuration
    /// </summary>
    public static IHttpClientBuilder AddTypedHttpClient<TClient, TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttpClient, TOptions>? configureClient = null)
        where TClient : class
        where TOptions : class, IHttpClientOptions
    {
        var httpClientOptions = configuration.GetRequiredOptions<HttpClientOptions>();

        services.ConfigureOptions<TOptions>(configuration);

        return services.AddHttpClient<TClient>((serviceProvider, client) =>
        {
            var options = configuration.GetRequiredOptions<TOptions>();

            if (!string.IsNullOrEmpty(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }

            client.Timeout = options.Timeout;

            client.AddDefaultRequestHeaders();

            configureClient?.Invoke(client, options);
        })
        .AddStandardPolicies(httpClientOptions);
    }

    /// <summary>
    /// Registers a typed HttpClient with interface-to-implementation mapping and options configuration
    /// </summary>
    public static IHttpClientBuilder AddTypedHttpClient<TInterface, TClient, TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttpClient, TOptions>? configureClient = null)
        where TInterface : class
        where TClient : class, TInterface
        where TOptions : class, IHttpClientOptions
    {
        var httpClientOptions = configuration.GetRequiredOptions<HttpClientOptions>();

        services.ConfigureOptions<TOptions>(configuration);

        return services.AddHttpClient<TInterface, TClient>((serviceProvider, client) =>
        {
            var options = configuration.GetRequiredOptions<TOptions>();

            if (!string.IsNullOrEmpty(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }

            client.Timeout = options.Timeout;

            client.AddDefaultRequestHeaders();

            configureClient?.Invoke(client, options);
        })
        .AddStandardPolicies(httpClientOptions);
    }

    /// <summary>
    /// Registers a typed HttpClient with a handler
    /// </summary>
    public static IHttpClientBuilder AddTypedHttpClientWithHandler<TClient, TOptions, THandler>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttpClient, TOptions>? configureClient = null)
        where TClient : class
        where TOptions : class, IHttpClientOptions
        where THandler : DelegatingHandler
    {
        services.TryAddTransient<THandler>();

        return AddTypedHttpClient<TClient, TOptions>(services, configuration, configureClient)
            .AddHttpMessageHandler<THandler>();
    }

    /// <summary>
    /// Registers a typed HttpClient with interface-to-implementation mapping and a handler
    /// </summary>
    [SuppressMessage("SonarAnalyzer.CSharp", "S2436:Reduce the number of generic parameters",
        Justification = "All four type parameters are required for typed HttpClient registration with handler support, following Microsoft's HttpClientFactory patterns.")]
    public static IHttpClientBuilder AddTypedHttpClientWithHandler<TInterface, TClient, TOptions, THandler>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<HttpClient, TOptions>? configureClient = null)
        where TInterface : class
        where TClient : class, TInterface
        where TOptions : class, IHttpClientOptions
        where THandler : DelegatingHandler
    {
        services.TryAddTransient<THandler>();

        return AddTypedHttpClient<TInterface, TClient, TOptions>(services, configuration, configureClient)
            .AddHttpMessageHandler<THandler>();
    }

    private static void AddDefaultRequestHeaders(this HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent", "ERP-WMS-Connector/1.0");
    }

    /// <summary>
    /// Adds configurable resilience policies to HttpClient
    /// </summary>
    public static IHttpClientBuilder AddStandardPolicies(this IHttpClientBuilder builder, HttpClientOptions httpClientOptions)
    {
        builder.AddStandardResilienceHandler(options =>
        {
            // Configure retry strategy
            // https://www.pollydocs.org/strategies/retry.html
            options.Retry.MaxRetryAttempts = httpClientOptions.Retry.MaxRetryAttempts;
            options.Retry.BackoffType = httpClientOptions.Retry.BackoffType;
            options.Retry.Delay = httpClientOptions.Retry.Delay;
            options.Retry.MaxDelay = httpClientOptions.Retry.MaxDelay;
            options.Retry.UseJitter = httpClientOptions.Retry.UseJitter;

            // Configure timeout strategy
            // https://www.pollydocs.org/strategies/timeout.html
            options.TotalRequestTimeout.Timeout = httpClientOptions.RequestTimeout.TotalRequestTimeout;
            options.AttemptTimeout.Timeout = httpClientOptions.RequestTimeout.AttemptTimeout;

            // Configure circuit breaker
            // https://www.pollydocs.org/strategies/circuit-breaker.html
            options.CircuitBreaker.SamplingDuration = httpClientOptions.CircuitBreaker.SamplingDuration;
            options.CircuitBreaker.FailureRatio = httpClientOptions.CircuitBreaker.FailureRatio;
            options.CircuitBreaker.MinimumThroughput = httpClientOptions.CircuitBreaker.MinimumThroughput;
            options.CircuitBreaker.BreakDuration = httpClientOptions.CircuitBreaker.BreakDuration;
        });

        return builder;
    }

    /// <summary>
    /// Adds custom resilience handler configuration to HttpClient
    /// </summary>
    public static IHttpClientBuilder AddCustomResiliencePolicies(this IHttpClientBuilder builder, Action<HttpStandardResilienceOptions> configureOptions)
    {
        builder.AddStandardResilienceHandler(configureOptions);

        return builder;
    }
}