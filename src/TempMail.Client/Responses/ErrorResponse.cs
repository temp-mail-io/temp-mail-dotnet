namespace TempMail.Client.Responses;

/// <summary>
/// Represents request that has finished with an error
/// </summary>
/// <param name="error">The error itself</param>
/// <param name="meta">Meta-information about the error</param>
public class ErrorResponse(Error error, ErrorMeta meta)
{
    /// <summary>
    /// The error itself
    /// </summary>
    public Error Error { get; } = error;
    
    /// <summary>
    /// Meta-information about the error
    /// </summary>
    public ErrorMeta Meta { get; } = meta;
}

/// <summary>
/// Meta-information about the error
/// </summary>
/// <param name="requestId">ID of the current request. May be used to get some info from the support.</param>
public class ErrorMeta(string requestId)
{
    /// <summary>
    /// ID of the current request. May be used to get some info from the support.
    /// </summary>
    public string RequestId { get; } = requestId;
}

/// <summary>
/// The error information
/// </summary>
/// <param name="type">Type of the error</param>
/// <param name="code">Error code</param>
/// <param name="detail">Error details</param>
public class Error(
    ErrorType type,
    string code,
    string detail)
{
    /// <summary>
    /// Type of the error
    /// </summary>
    public ErrorType Type { get; } = type;
    
    /// <summary>
    /// Error code. Some common codes are <c>not_found</c>, <c>invalid_api_key</c>, <c>rate_limited</c>, <c>validation_error</c>.
    /// </summary>
    public string Code { get; } = code;
    
    /// <summary>
    /// Error details
    /// </summary>
    public string Detail { get; } = detail;
}

/// <summary>
/// Type of the encountered error
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Indicates that there was an error with the sent request
    /// </summary>
    RequestError = 0,
    
    /// <summary>
    /// Indicates that there was an error during the request handling
    /// </summary>
    ApiError = 1
}