using System;
using TempMail.Client.Responses;

namespace TempMail.Client;

public class TempMailClientException : Exception
{
    public ErrorResponse? Error { get; }
    
    public byte[]? ErrorData { get; }
    
    public TempMailClientException(ErrorResponse error) : base(
        $"Error of type {error.Error.Type} ({error.Error.Code}) occurred: {error.Error.Detail}. " +
        $"Try contacting support with the request ID: {error.Meta.RequestId}")
    {
        Error = error;
    }

    public TempMailClientException(byte[] errorContent) : base(
        $"Something went wrong during request processing. Response status code was not 200 (OK), but content is not {nameof(ErrorResponse)}.")
    {
        ErrorData = errorContent;
    }
}