using TempMail.Client.Models;
using TempMail.Client.Requests;

namespace TempMail.Client.Tests.Util;

public class InMemoryMailboxManager
{
    public Dictionary<string, Mailbox> Mailboxes { get; } = [];

    public Dictionary<string, Message> Messages { get; } = [];
    
    public Dictionary<string, byte[]> Attachments { get; } = [];

    public Dictionary<DomainType, string> Domains { get; } = new()
    {
        { DomainType.Custom, "custom.com" },
        { DomainType.Public, "public.com" },
        { DomainType.Premium, "premium.com" }
    };
    
    public RateLimitStatus RateLimitStatus { get; } = new(
        int.MaxValue,
        int.MaxValue,
        int.MaxValue,
        Convert.ToInt32(DateTime.UtcNow.AddDays(30).Subtract(DateTime.UnixEpoch).TotalSeconds)); 


    public string CreateEmail(CreateEmailRequest request)
    {
        switch (request)
        {
            case CreateEmailByEmailRequest { Email: var email }:
                if (Mailboxes.TryGetValue(email, out _))
                {
                    throw new Exception($"Email already exists: {email}");
                }
                Mailboxes.Add(email, new Mailbox(email));
                return email;
            
            case CreateEmailByDomainRequest { Domain: var domain }:
                var byDomainEmail = $"{Guid.NewGuid():N}@{domain}";
                if (Mailboxes.TryGetValue(byDomainEmail, out _))
                {
                    throw new Exception($"Email already exists: {byDomainEmail}");
                }
                Mailboxes.Add(byDomainEmail, new Mailbox(byDomainEmail));
                return byDomainEmail;
            
            case CreateEmailByDomainTypeRequest { DomainType: var domainType }:
                var byDomainTypeEmail = $"{Guid.NewGuid():N}@{Domains[domainType]}";
                if (Mailboxes.TryGetValue(byDomainTypeEmail, out _))
                {
                    throw new Exception($"Email already exists: {byDomainTypeEmail}");
                }
                Mailboxes.Add(byDomainTypeEmail, new Mailbox(byDomainTypeEmail));
                return byDomainTypeEmail;
            
            default:
                throw new Exception($"Unknown email type: {request}");
        }
    }

    public Message[] GetAllMessages(string email) => Mailboxes[email].Messages.Values.ToArray();
    
    public void DeleteEmail(string email) => Mailboxes.Remove(email);
    
    public Message GetSpecificMessage(string id) => Messages[id];

    public void DeleteSpecificMessage(string id)
    {
        Messages.Remove(id);

        foreach (var mailbox in Mailboxes.Values)
        {
            mailbox.Messages.Remove(id);
        }
    }

    public string GetMessageSourceCode(string messageId) => Messages[messageId].BodyHtml;
    
    public byte[] GetAttachment(string attachmentId) => Attachments[attachmentId];
    
    public Domain[] GetAvailableDomains() => Domains
        .Select(x => new Domain(x.Value, x.Key))
        .ToArray();
    
    public void AddMessage(Message message)
    {
        if (!Messages.TryAdd(message.Id, message))
        {
            throw new InvalidOperationException($"Message already exists: {message.Id}");
        }

        if (Mailboxes.TryGetValue(message.From, out var fromMailbox))
        {
            fromMailbox.Messages.Add(message.Id, message);
        }

        if (Mailboxes.TryGetValue(message.To, out var toMailbox))
        {
            toMailbox.Messages.Add(message.Id, message);
        }
    }

    public void AddAttachment(string emailFrom, string emailTo, string messageId, byte[] attachment)
    {
        var attachmentId = Guid.NewGuid().ToString();
        var newMessage = Messages[messageId] = new Message(
            Messages[messageId].Id,
            Messages[messageId].From,
            Messages[messageId].To,
            Messages[messageId].Cc,
            Messages[messageId].Subject,
            Messages[messageId].BodyText,
            Messages[messageId].BodyHtml,
            Messages[messageId].CreatedAt,
            Messages[messageId].Attachments
                .Concat([new Attachment(attachmentId, attachmentId, attachment.Length)]).ToArray());

        if (Mailboxes.TryGetValue(emailFrom, out var mailboxFrom))
        {
            mailboxFrom.Messages[messageId] = newMessage;
        }

        if (Mailboxes.TryGetValue(emailTo, out var mailboxTo))
        {
            mailboxTo.Messages[messageId] = newMessage;
        }

        Attachments.Add(attachmentId, attachment);
    }

}

public class Mailbox(string email)
{
    public string Email { get; } = email;

    public Dictionary<string, Message> Messages { get; set; } = [];
}