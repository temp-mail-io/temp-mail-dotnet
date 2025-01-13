using TempMail.Client.Models;

namespace TempMail.Client.Responses;

public class GetAllMessagesResponse(Message[] messages)
{
    public Message[] Messages { get; } = messages;
}