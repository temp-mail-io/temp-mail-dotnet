namespace TempMail.Client.Responses;

public class Response<TSuccess>
{
    internal Response() {}
    
    public bool IsSuccess { get; internal set; }
    public TSuccess? Result { get; internal set; }
    public ErrorResponse? ErrorResult { get; internal set; }
}

public static class Response
{
    public static Response<T> Success<T>(T result) => new()
    {
        IsSuccess = true,
        Result = result
    };

    public static Response<T> Error<T>(ErrorResponse? errorResult) => new()
    {
        IsSuccess = false,
        ErrorResult = errorResult
    };
}