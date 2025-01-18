using System;

using TempMail.Client.Responses;

namespace TempMail.Client.Exceptions;

/// <summary>
/// Represents unexpected response from the <a href="https://api.temp-mail.io">API</a>
/// </summary>
/// <param name="errorContent">The content read from the repsonse</param>
public class TempMailClientUnexpectedResponseException(byte[] errorContent) : Exception(
    $"Something went wrong during request processing. Response status code was not 200 (OK), but content is not {nameof(ErrorResponse)}.")
{
    /// <summary>
    /// The content read from the repsonse
    /// </summary>
    public byte[] ErrorData { get; } = errorContent;
}