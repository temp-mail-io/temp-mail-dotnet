using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client;

/// <summary>
/// <inheritdoc cref="ITempMailClient"/>
/// </summary>
public class TempMailClient : ITempMailClient
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
        }
    };
        
    private TempMailClient(
        TempMailClientConfiguration config,
        HttpClient httpClient,
        bool disposeHttpClient)
    {
        _httpClient = httpClient;
        _disposeHttpClient = disposeHttpClient;
        _httpClient.BaseAddress = new Uri(config.ApiUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", config.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", $".NET temp-mail.io client (Library version: {config.ClientVersion}; .NET Runtime: {config.DotnetRuntimeVersion}; OS: {config.OsVersion})");
    }

    /// <summary>
    /// Create a <see cref="TempMailClient"/>. This will create an <see cref="HttpClient"/> internally.
    /// </summary>
    /// <param name="config"><see cref="TempMailClientConfiguration"/></param>
    /// <returns><see cref="TempMailClient"/></returns>
    public static TempMailClient Create(TempMailClientConfiguration config) => 
        Create(config, new HttpClient(), true);

    /// <summary>
    /// Create a <see cref="TempMailClient"/> using existing <see cref="HttpClient"/>
    /// </summary>
    /// <param name="config"><see cref="TempMailClientConfiguration"/></param>
    /// <param name="httpClient">An existing <see cref="HttpClient"/> to be used by this instance of <see cref="TempMailClient"/></param>
    /// <param name="disposeHttpClient">Tells whether to dispose of the specified <paramref name="httpClient"/>, <c>false</c> by default</param>
    /// <returns><see cref="TempMailClient"/></returns>
    /// <remarks>
    /// This method is useful in combination with ASP .NET <c>IHttpClientFactory</c>.
    /// For other scenarios the <paramref name="disposeHttpClient"/> parameter was added.
    /// </remarks>
    public static TempMailClient Create(
        TempMailClientConfiguration config,
        HttpClient httpClient,
        bool disposeHttpClient = false) =>
        new(config, httpClient, disposeHttpClient);

    private async Task<Response<TResponse>> SendHttpRequest<TResponse>(HttpRequestMessage msg, CancellationToken ct)
    {
        var response = await _httpClient
            .SendAsync(msg, ct)
            .ConfigureAwait(false);
        
        if (!response.IsSuccessStatusCode)
        {
            await using var errorResponseContentStream = await response.Content
                .ReadAsStreamAsync()
                .ConfigureAwait(false);
            
            var errorResponseContent = await JsonSerializer
                .DeserializeAsync<ErrorResponse>(
                    errorResponseContentStream,
                    JsonOptions,
                    ct)
                .ConfigureAwait(false);
            
            return Response.Error<TResponse>(errorResponseContent);
        }
        
        if (typeof(TResponse) == typeof(EmptyResponse))
        {
            return Unsafe.As<Response<TResponse>>(Response.Success(new EmptyResponse()));
        }

        if (typeof(TResponse) == typeof(string))
        {
            var responseContentString = await response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
            
            return Unsafe.As<Response<TResponse>>(Response.Success(responseContentString));
        }
        
        await using var responseContentStream = await response.Content
            .ReadAsStreamAsync()
            .ConfigureAwait(false);
        
        var responseContent = await JsonSerializer
            .DeserializeAsync<TResponse>(
                responseContentStream,
                JsonOptions,
                ct)
            .ConfigureAwait(false);
        
        return Response.Success(responseContent!);
    }

    private async Task<Response<TResponse>> SendRequest<TRequest, TResponse>(
        TRequest request,
        string url,
        HttpMethod method,
        CancellationToken ct = default) 
    {
        var stringRequestContent = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(stringRequestContent, Encoding.UTF8, "application/json");
        using var msg = new HttpRequestMessage(method, url);
        msg.Content = content;
        return await SendHttpRequest<TResponse>(msg, ct)
            .ConfigureAwait(false);
    }

    private async Task<Response<TResponse>> SendRequestNoContent<TResponse>(
        string url,
        HttpMethod method,
        CancellationToken ct = default)
    {
        using var msg = new HttpRequestMessage(method, url);
        return await SendHttpRequest<TResponse>(msg, ct)
            .ConfigureAwait(false);
    }

    public Task<Response<CreateEmailResponse>> CreateEmail(
        CreateEmailRequest request,
        CancellationToken ct = default) =>
        SendRequest<CreateEmailRequest, CreateEmailResponse>(request, "/v1/emails", HttpMethod.Post, ct);

    public Task<Response<GetAllMessagesResponse>> GetAllMessages(
        GetAllMessagesRequest request,
        CancellationToken ct = default) =>
        SendRequestNoContent<GetAllMessagesResponse>($"/v1/emails/{request.Email}/messages", HttpMethod.Get, ct);
    
    public Task<Response<EmptyResponse>> DeleteEmail(
        DeleteEmailRequest request,
        CancellationToken ct = default) =>
        SendRequestNoContent<EmptyResponse>($"/v1/emails/{request.Email}", HttpMethod.Delete, ct);

    public Task<Response<Message>> GetSpecificMessage(
        GetSpecificMessageRequest request,
        CancellationToken ct = default) =>
        SendRequestNoContent<Message>($"/v1/messages/{request.Id}", HttpMethod.Get, ct);

    public Task<Response<EmptyResponse>> DeleteSpecificMessage(
        DeleteSpecificMessageRequest request,
        CancellationToken ct = default) =>
        SendRequestNoContent<EmptyResponse>($"/v1/messages/{request.Id}", HttpMethod.Delete, ct);

    public Task<Response<string>> GetMessageSourceCode(
        GetMessageSourceCodeRequest request,
        CancellationToken ct = default) =>
        SendRequestNoContent<string>($"/v1/messages/{request.Id}/source_code", HttpMethod.Get, ct);

    public Task<Response<string>> GetAttachment(
        GetAttachmentRequest request,
        CancellationToken ct = default) =>
        SendRequestNoContent<string>($"/v1/attachments/{request.Id}", HttpMethod.Get, ct);

    public Task<Response<GetAvailableDomainsResponse>> GetAvailableDomains(
        CancellationToken ct = default) =>
        SendRequestNoContent<GetAvailableDomainsResponse>($"/v1/domains", HttpMethod.Get, ct);

    public Task<Response<RateLimitStatus>> GetRateLimitStatus(
        CancellationToken ct = default) =>
        SendRequestNoContent<RateLimitStatus>($"/v1/rate_limit", HttpMethod.Get, ct);

    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}