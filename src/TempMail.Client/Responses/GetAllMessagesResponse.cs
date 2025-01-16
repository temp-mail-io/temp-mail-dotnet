using TempMail.Client.Models;

namespace TempMail.Client.Responses;

/// <summary>
/// Response to <see cref="Requests.GetAllMessagesRequest"/>
/// </summary>
/// <param name="messages">Array of found <see cref="Message"/>s</param>
public class GetAllMessagesResponse(Message[] messages)
{
    /// <summary>
    /// Array of found <see cref="Message"/>s
    /// </summary>
    public Message[] Messages { get; } = messages;
}