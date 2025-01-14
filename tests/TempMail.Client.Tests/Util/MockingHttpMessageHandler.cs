using System.Net;
using System.Reflection;
using System.Text.Json;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests.Util;

internal class MockingHttpMessageHandler : HttpMessageHandler
{
    private static MockingHttpMessageHandler? _instance;
    
    private bool _returnErrors;
    private readonly InMemoryMailboxManager _mailboxManager;
    

    public MockingHttpMessageHandler(InMemoryMailboxManager mailboxManager)
    {
        _mailboxManager = mailboxManager;
        if (_instance != null)
        {
            throw new InvalidOperationException("MockingHttpMessageHandler instance already exists");
        }
        
        _instance = this;
    }


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
        if (_returnErrors)
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

    [Handler(".*/v1/emails", "POST")]
    private async Task<HttpResponseMessage> HandleCreateEmail(HttpRequestMessage request)
    {
        var requestBody = JsonSerializer.Deserialize<CreateEmailRequest>(
            await request.Content.ReadAsStringAsync(),
            JsonOptions);
        
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        
        var isSuccess = TryMakeRequest(() => _mailboxManager.CreateEmail(requestBody!), out var email);

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

    [Handler(@".*/v1/emails/(.*)/messages", "GET")]
    private async Task<HttpResponseMessage> HandleGetAllMessage(HttpRequestMessage _, string email)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        
        var isSuccess = TryMakeRequest(() => _mailboxManager.GetAllMessages(email), out var messages);
        
        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = messages.Content;
            return httpResponseMessage;
        }

        var resp = new GetAllMessagesResponse(messages.Result);
        
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(resp, JsonOptions));
        
        return httpResponseMessage;
    }

    [Handler(@".*/v1/emails/(.*)", "DELETE")]
    private async Task<HttpResponseMessage> HandleDeleteEmail(HttpRequestMessage _, string email)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        var isSuccess = TryMakeRequest(() =>
            {
                _mailboxManager.DeleteEmail(email);
                return string.Empty;
            },
            out var content);
        
        if (!isSuccess)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
            httpResponseMessage.Content = content.Content;
            return httpResponseMessage;
        }
        
        return httpResponseMessage;
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
            return (Task<HttpResponseMessage>)methodInfo.Invoke(_instance, [request])!;
        }
        
        return (Task<HttpResponseMessage>)methodInfo.Invoke(_instance, [request, match])!;
    };
    
    public void ReturnErrors(bool returnErrors = true) => _returnErrors = returnErrors;

    public new void Dispose()
    {
        _instance = null!;
        base.Dispose();
    }
}