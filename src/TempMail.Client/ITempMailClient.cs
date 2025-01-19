using System;
using System.Threading;
using System.Threading.Tasks;

using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client;

/// <summary>
/// A <a href="https://temp-mail.io">temp-mail</a> client contract
/// </summary>
public interface ITempMailClient : IDisposable
{
    /// <summary>
    /// Create an e-mail box
    /// </summary>
    /// <param name="request"><see cref="CreateEmailRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="CreateEmailResponse"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<CreateEmailResponse>> CreateEmail(
        CreateEmailRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Get all the messages in the given e-mail box
    /// </summary>
    /// <param name="request"><see cref="GetAllMessagesRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="GetAllMessagesResponse"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<GetAllMessagesResponse>> GetAllMessages(
        GetAllMessagesRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Delete given e-mail box
    /// </summary>
    /// <param name="request"><see cref="DeleteEmailRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="EmptyResponse"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<EmptyResponse>> DeleteEmail(
        DeleteEmailRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Get specific e-mail message by ID
    /// </summary>
    /// <param name="request"><see cref="GetSpecificMessageRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="Message"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<Message>> GetSpecificMessage(
        GetSpecificMessageRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Delete specific e-mail message
    /// </summary>
    /// <param name="request"><see cref="DeleteSpecificMessageRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="EmptyResponse"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<EmptyResponse>> DeleteSpecificMessage(
        DeleteSpecificMessageRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Get specific e-mail message source code
    /// </summary>
    /// <param name="request"><see cref="GetMessageSourceCodeRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="string"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<string>> GetMessageSourceCode(
        GetMessageSourceCodeRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Get attachment to the given e-mail message 
    /// </summary>
    /// <param name="request"><see cref="GetAttachmentRequest"/></param>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="T:byte[]"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<byte[]>> GetAttachment(
        GetAttachmentRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Get all the domains that are available to the currently used API-key
    /// </summary>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="GetAvailableDomainsResponse"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<GetAvailableDomainsResponse>> GetAvailableDomains(CancellationToken ct = default);

    /// <summary>
    /// Get rate limit status of the currently used API-key
    /// </summary>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Response{TSuccess}"/>, either successful <see cref="RateLimitStatus"/> or <see cref="ErrorResponse"/></returns>
    Task<Response<RateLimitStatus>> GetRateLimitStatus(CancellationToken ct = default);
}