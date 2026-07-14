using Erp.WmsConnector.Shared.Abstractions.Options;
using Polly;

namespace Erp.WmsConnector.Shared.Infrastructure.HttpClients;

public sealed class HttpClientOptions : IOptions
{
    public static string SectionName => "HttpClient";

    public RetryOptions Retry { get; set; } = new();
    public TimeoutOptions RequestTimeout { get; set; } = new();
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();

    public sealed class RetryOptions
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public DelayBackoffType BackoffType { get; set; } = DelayBackoffType.Exponential;
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
        public double BackoffMultiplier { get; set; } = 2.0;
        public bool UseJitter { get; set; } = true;
    }

    public sealed class TimeoutOptions
    {
        public TimeSpan TotalRequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan AttemptTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }

    public sealed class CircuitBreakerOptions
    {
        public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);
        public double FailureRatio { get; set; } = 0.5;
        public int MinimumThroughput { get; set; } = 3;
        public TimeSpan BreakDuration { get; set; } = TimeSpan.FromSeconds(5);
        public int SlidingWindowSize { get; set; } = 100;
    }
}
