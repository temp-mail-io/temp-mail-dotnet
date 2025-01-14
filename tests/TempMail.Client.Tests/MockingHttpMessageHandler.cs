using System.Net;
using System.Reflection;
using System.Text.Json;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

internal class MockingHttpMessageHandler : HttpMessageHandler
{
    private bool _returnErrors;

    private static MockingHttpMessageHandler? _instance;

    public MockingHttpMessageHandler()
    {
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
                .Select(x => 
                    (x.GetCustomAttribute<HandlerAttribute>()!, CreateMethod(x)))
                .Where(x => x.Item1 != null)
                .ToList();
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
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

    private HttpResponseMessage ReturnError<TResponse>()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(
            new ErrorResponse(
                new Error(ErrorType.ApiError, "api_error", "An error occured during request handling"),
                new ErrorMeta(Guid.NewGuid().ToString())),
            JsonOptions));

        return httpResponseMessage;
    }

    [Handler(".*/v1/emails", "POST")]
    private async Task<HttpResponseMessage> HandleCreateEmail(HttpRequestMessage request)
    {
        if (_returnErrors)
        {
            return ReturnError<HttpResponseMessage>();
        }
        var requestBody = JsonSerializer.Deserialize<CreateEmailRequest>(
            await request.Content.ReadAsStringAsync(),
            JsonOptions);
        
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var resp = requestBody switch
        {
            CreateEmailByEmailRequest { Email: var email } => new CreateEmailResponse(email, int.MaxValue),
            CreateEmailByDomainRequest { Domain: var domain } => new CreateEmailResponse($"random@{domain}", int.MaxValue),
            CreateEmailByDomainTypeRequest { DomainType: var domainType } => new CreateEmailResponse($"random@{domainType.ToString().ToLowerInvariant()}.io", int.MaxValue),
            _ => throw new Exception($"Unknown request body type: {requestBody?.GetType().FullName}")
        };
        httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(resp, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new CreateEmailRequestJsonConverter() }
        }));
        return httpResponseMessage;
    }

    private static JsonSerializerOptions JsonOptions =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new CreateEmailRequestJsonConverter() }
        };

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