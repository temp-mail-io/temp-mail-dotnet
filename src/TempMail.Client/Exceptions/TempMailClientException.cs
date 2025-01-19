using System;

using TempMail.Client.Responses;

namespace TempMail.Client.Exceptions;

/// <summary>
/// Specific exception for the library
/// </summary>
/// <param name="error"></param>
public class TempMailClientException(ErrorResponse error) : Exception(
    $"Error of type {error.Error.Type} ({error.Error.Code}) occurred: {error.Error.Detail}. " +
        $"Try contacting support with the request ID: {error.Meta.RequestId}")
{
    /// <summary>
    /// Error response from the <a href="https://api.temp-mail.io">API</a>
    /// </summary>
    public ErrorResponse Error { get; } = error;
}