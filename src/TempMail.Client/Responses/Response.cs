namespace TempMail.Client.Responses;

/// <summary>
/// Representation of any of the <a href="https://temp-mail.io">temp-mail</a> response
/// </summary>
/// <typeparam name="TSuccess">Type, representing the successful response</typeparam>
/// <remarks>
/// Can be created with non-generic <see cref="Response"/> static type.
/// </remarks>
public class Response<TSuccess>
{
    internal Response() {}
    
    /// <summary>
    /// Tells whether the request has finished successfully
    /// </summary>
    public bool IsSuccess { get; internal set; }
    
    /// <summary>
    /// Successful response, not <c>null</c> if <see cref="IsSuccess"/> is <c>true</c>, otherwise - <c>null</c>
    /// </summary>
    public TSuccess? Result { get; internal set; }
    
    /// <summary>
    /// Error response, not <c>null</c> if <see cref="IsSuccess"/> is <c>false</c>, otherwise - <c>null</c>
    /// </summary>
    public ErrorResponse? ErrorResult { get; internal set; }

    public void ThrowIfError()
    {
        if (!IsSuccess)
        {
            throw new TempMailClientException(ErrorResult!);
        }
    }
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