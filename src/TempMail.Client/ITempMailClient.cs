using System;
using System.Threading;
using System.Threading.Tasks;
using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client;

public interface ITempMailClient : IDisposable
{
    Task<Response<CreateEmailResponse>> CreateEmail(
        CreateEmailRequest request,
        CancellationToken ct = default);

    Task<Response<GetAllMessagesResponse>> GetAllMessages(
        GetAllMessagesRequest request,
        CancellationToken ct = default);
    
    Task<Response<EmptyResponse>> DeleteEmail(
        DeleteEmailRequest request,
        CancellationToken ct = default);

    Task<Response<Message>> GetSpecificMessage(
        GetSpecificMessageRequest request,
        CancellationToken ct = default);

    Task<Response<EmptyResponse>> DeleteSpecificMessage(
        DeleteSpecificMessageRequest request,
        CancellationToken ct = default);

    Task<Response<Message>> GetMessageSourceCode(
        GetMessageSourceCodeRequest request,
        CancellationToken ct = default);
    
    Task<Response<string>> GetAttachment(
        GetAttachmentRequest request,
        CancellationToken ct = default);

    Task<Response<GetAvailableDomainsResponse>> GetAvailableDomains(CancellationToken ct = default);

    Task<Response<RateLimitStatus>> GetRateLimitStatus(CancellationToken ct = default);
}