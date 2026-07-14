using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Erp.WmsConnector.Shared.Infrastructure.HttpClients;

/// <summary>
/// Base class for typed HTTP clients providing common HTTP operations
/// </summary>
public abstract class BaseHttpClient(HttpClient httpClient)
{
    protected readonly HttpClient HttpClient = httpClient;

    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    #region GET Operations

    protected virtual async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Performs a GET request with custom error handling
    /// </summary>
    /// <param name="endpoint">The endpoint to call</param>
    /// <param name="errorMessageSelector">Function to extract error message from error response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task<TResponse?> GetWithErrorHandlingAsync<TResponse, TError>(
        string endpoint,
        Func<TError, string?> errorMessageSelector,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.GetAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowHttpExceptionWithErrorBodyAsync(response, errorMessageSelector, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    #endregion

    #region POST Operations

    protected virtual async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(endpoint, request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Performs a POST request with custom error handling
    /// </summary>
    /// <param name="endpoint">The endpoint to call</param>
    /// <param name="request">The request body</param>
    /// <param name="errorMessageSelector">Function to extract error message from error response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task<TResponse?> PostWithErrorHandlingAsync<TRequest, TResponse, TError>(
        string endpoint,
        TRequest request,
        Func<TError, string?> errorMessageSelector,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(endpoint, request, JsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowHttpExceptionWithErrorBodyAsync(response, errorMessageSelector, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    #endregion

    #region PUT Operations

    protected virtual async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.PutAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Performs a PUT request with custom error handling
    /// </summary>
    /// <param name="endpoint">The endpoint to call</param>
    /// <param name="request">The request body</param>
    /// <param name="errorMessageSelector">Function to extract error message from error response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task<TResponse?> PutWithErrorHandlingAsync<TRequest, TResponse, TError>(
        string endpoint,
        TRequest request,
        Func<TError, string?> errorMessageSelector,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await HttpClient.PutAsJsonAsync(endpoint, request, JsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowHttpExceptionWithErrorBodyAsync(response, errorMessageSelector, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
    }

    protected virtual async Task<HttpResponseMessage> PutAsync(string endpoint, string content, string mediaType = "application/json", CancellationToken cancellationToken = default)
    {
        using var stringContent = new StringContent(content, Encoding.UTF8, mediaType);
        HttpResponseMessage response = await HttpClient.PutAsync(endpoint, stringContent, cancellationToken);

        return response;
    }

    #endregion

    #region Other Operations

    protected virtual async Task<HttpResponseMessage> HeadAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, endpoint);
        HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);

        return response;
    }

    protected virtual bool IsSuccessResponse(HttpResponseMessage response, string endpoint)
    {
        return response.IsSuccessStatusCode;
    }

    #endregion

    #region Error Handling Helpers

    /// <summary>
    /// Extracts error message from response body and throws HttpRequestException
    /// </summary>
    /// <param name="response">The HTTP response</param>
    /// <param name="errorMessageSelector">Function to extract error message from error response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="HttpRequestException">Always thrown with extracted error message</exception>
    protected static async Task ThrowHttpExceptionWithErrorBodyAsync<TError>(
        HttpResponseMessage response,
        Func<TError, string?> errorMessageSelector,
        CancellationToken cancellationToken = default)
    {
        string errorMessage = await ExtractErrorMessageAsync(response, errorMessageSelector, cancellationToken);
        throw new HttpRequestException(errorMessage, null, response.StatusCode);
    }

    /// <summary>
    /// Extracts error message from HTTP response body
    /// </summary>
    /// <param name="response">The HTTP response</param>
    /// <param name="errorMessageSelector">Function to extract error message from error response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The extracted error message or raw response content if deserialization fails</returns>
    protected static async Task<string> ExtractErrorMessageAsync<TError>(
        HttpResponseMessage response,
        Func<TError, string?> errorMessageSelector,
        CancellationToken cancellationToken = default)
    {
        try
        {
            TError? errorResponse = await response.Content.ReadFromJsonAsync<TError>(JsonOptions, cancellationToken);
            if (errorResponse is not null)
            {
                string? message = errorMessageSelector(errorResponse);
                if (!string.IsNullOrEmpty(message))
                {
                    return message;
                }
            }
        }
        catch
        {
            // Deserialization failed, fall back to raw content
        }

        string rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(rawContent)
            ? $"HTTP request failed with status code {(int)response.StatusCode} ({response.StatusCode})"
            : rawContent;
    }

    #endregion
}