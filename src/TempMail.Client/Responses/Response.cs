using TempMail.Client.Exceptions;

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
    internal Response() { }

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

    /// <summary>
    /// Throw <see cref="TempMailClientException"/> if <see cref="IsSuccess"/> is <c>false</c>
    /// </summary>
    /// <exception cref="TempMailClientException"></exception>
    public void ThrowIfError()
    {
        if (!IsSuccess)
        {
            throw new TempMailClientException(ErrorResult!);
        }
    }
}

/// <summary>
/// Contains method for <see cref="Response{TSuccess}"/> instance creation
/// </summary>
public static class Response
{
    /// <summary>
    /// Create an instance of <see cref="Response{TSuccess}"/> with <see cref="Response{TSuccess}.IsSuccess"/> is <c>true</c>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns>Instance of <see cref="Response{TSuccess}"/></returns>
    public static Response<T> Success<T>(T result) => new()
    {
        IsSuccess = true,
        Result = result
    };

    /// <summary>
    /// Create an instance of <see cref="Response{TSuccess}"/> with <see cref="Response{TSuccess}.IsSuccess"/> is <c>false</c>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="errorResult"></param>
    /// <returns></returns>
    public static Response<T> Error<T>(ErrorResponse? errorResult) => new()
    {
        IsSuccess = false,
        ErrorResult = errorResult
    };
}