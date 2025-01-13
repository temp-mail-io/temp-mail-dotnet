namespace TempMail.Client.Responses;

public class ErrorResponse(Error error, ErrorMeta meta)
{
    public Error Error { get; } = error;
    public ErrorMeta Meta { get; } = meta;
}

public class ErrorMeta(string requestId)
{
    public string RequestId { get; } = requestId;
}

public class Error(
    ErrorType type,
    string code,
    string detail)
{
    public ErrorType Type { get; } = type;
    public string Code { get; } = code;
    public string Detail { get; } = detail;
}

public enum ErrorType
{
    RequestError = 0,
    ApiError = 1
}