using System.Net;
using System.Reflection;
using System.Text.Json;

using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests.Util;

internal class MockingHttpMessageHandler : HttpMessageHandler
{
    private static MockingHttpMessageHandler? instance;

    private bool returnErrors;
    private readonly InMemoryMailboxManager mailboxManager;


    public MockingHttpMessageHandler(InMemoryMailboxManager mailboxManager)
    {
        this.mailboxManager = mailboxManager;
        if (instance != null)
        {
            throw new InvalidOperationException("MockingHttpMessageHandler instance already exists");
        }

        instance = this;
    }


    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    private static readonly IReadOnlyCollection<(HandlerAttribute Handler, Func<HttpRequestMessage, Task<HttpResponseMessage>> Method)>
        Handlers =
            typeof(MockingHttpMessageHandler)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Select(x => (x.GetCustomAttribute<HandlerAttribute>()!, CreateMethod(x)))
                .Where(x => x.Item1 != null)
                .ToList();

    private static JsonSerializerOptions JsonOptions =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new CreateEmailRequestJsonConverter() }
        };

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (returnErrors)
        {
            return Task.FromResult(ReturnError());
        }

        foreach (var handler in Handlers)
        {
            if (handler.Handler.Matches(request) == null)
            {
                continue;
            }

            return handler.Method(request);
        }

        throw new Exception($"No handler found for {request.RequestUri}");
    }

    [Handler("^.*/v1/emails$", "POST")]
    private async Task<HttpResponseMessage> HandleCreateEmail(HttpRequestMessage request)
    {
        var requestBody = JsonSerializer.Deserialize<CreateEmailRequest>(
            await request.Content!.ReadAsStringAsync(),
            JsonOptions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.CreateEmail(requestBody!), out var email);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = email.Content;
            return httpResponseMessage;
        }

        var resp = new CreateEmailResponse(email.Result, int.MaxValue);

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(resp, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new CreateEmailRequestJsonConverter() }
        }));
        return httpResponseMessage;
    }

    [Handler(@"^.*/v1/emails/(.*)/messages$", "GET")]
    private Task<HttpResponseMessage> HandleGetAllMessage(HttpRequestMessage _, string email)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.GetAllMessages(email), out var messages);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = messages.Content;
            return Task.FromResult(httpResponseMessage);
        }

        var resp = new GetAllMessagesResponse(messages.Result);

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(resp, JsonOptions));

        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/emails/(.*)$", "DELETE")]
    private Task<HttpResponseMessage> HandleDeleteEmail(HttpRequestMessage _, string email)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.DeleteEmail(email), out var content);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = content;
            return Task.FromResult(httpResponseMessage);
        }

        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/messages/(.*)$", "GET")]
    private Task<HttpResponseMessage> HandleGetSpecificMessage(HttpRequestMessage _, string messageId)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.GetSpecificMessage(messageId), out var message);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = message.Content;
            return Task.FromResult(httpResponseMessage);
        }

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(message.Result, JsonOptions));
        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/messages/(.*)$", "DELETE")]
    private Task<HttpResponseMessage> HandleDeleteSpecificMessage(HttpRequestMessage _, string messageId)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.DeleteSpecificMessage(messageId), out var content);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = content;
            return Task.FromResult(httpResponseMessage);
        }

        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/messages/(.*)/source_code$", "GET")]
    private Task<HttpResponseMessage> HandleGetMessageSourceCode(HttpRequestMessage _, string messageId)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.GetMessageSourceCode(messageId), out var content);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = content.Content;
            return Task.FromResult(httpResponseMessage);
        }

        httpResponseMessage.Content = new StringContent(content.Result);

        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/attachments/(.*)$", "GET")]
    private Task<HttpResponseMessage> HandleGetAttachment(HttpRequestMessage _, string attachmentId)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() => mailboxManager.GetAttachment(attachmentId), out var content);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = content.Content;
            return Task.FromResult(httpResponseMessage);
        }

        httpResponseMessage.Content = new StreamContent(new MemoryStream(content.Result));

        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/domains$", "GET")]
    private Task<HttpResponseMessage> HandleGetAvailableDomains(HttpRequestMessage _)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(mailboxManager.GetAvailableDomains, out var content);

        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = content.Content;
            return Task.FromResult(httpResponseMessage);
        }

        var getAvailableDomainsResponse = new GetAvailableDomainsResponse(content.Result);
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(getAvailableDomainsResponse, JsonOptions));

        return Task.FromResult(httpResponseMessage);
    }

    [Handler("^.*/v1/rate_limit$", "GET")]
    private Task<HttpResponseMessage> HandleGetRateLimitStatus(HttpRequestMessage _)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(
            mailboxManager.RateLimitStatus,
            JsonOptions));

        return Task.FromResult(httpResponseMessage);
    }

    /// <summary>
    /// sets <paramref name="result"/> only if the delegate has thrown
    /// </summary>
    private bool TryMakeRequest<T>(Func<T> f, out (HttpContent Content, T Result) result)
    {
        try
        {
            var res = f();
            result = (default, res)!;
            return true;
        }
        catch (Exception e)
        {
            var response = new ErrorResponse(
                new Error(ErrorType.ApiError, "api_error", e.ToString()),
                new ErrorMeta(Guid.NewGuid().ToString()));

            result = (new StringContent(JsonSerializer.Serialize(response, JsonOptions)), default)!;
            return false;
        }
    }

    /// <summary>
    /// sets <paramref name="content"/> only if the delegate has thrown
    /// </summary>
    private bool TryMakeRequest(Action f, out HttpContent content)
    {
        var isSuccess = TryMakeRequest(() =>
        {
            f();
            return string.Empty;
        }, out var result);

        content = result.Content;
        return isSuccess;
    }

    private HttpResponseMessage ReturnError()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(
            new ErrorResponse(
                new Error(ErrorType.ApiError, "api_error", "An error occured during request handling"),
                new ErrorMeta(Guid.NewGuid().ToString())),
            JsonOptions));

        return httpResponseMessage;
    }

    private static Func<HttpRequestMessage, Task<HttpResponseMessage>> CreateMethod(MethodInfo methodInfo) => request =>
    {
        var handler = methodInfo.GetCustomAttribute<HandlerAttribute>()!;
        var match = handler.Matches(request)!;

        if (match == string.Empty)
        {
            return (Task<HttpResponseMessage>)methodInfo.Invoke(instance, [request])!;
        }

        return (Task<HttpResponseMessage>)methodInfo.Invoke(instance, [request, match])!;
    };

    public void ReturnErrors(bool enable = true) => returnErrors = enable;

    public new void Dispose()
    {
        instance = null!;
        base.Dispose();
    }
}