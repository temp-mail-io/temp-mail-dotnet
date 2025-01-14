using TempMail.Client.Models;
using TempMail.Client.Requests;

namespace TempMail.Client.Tests.Util;

public class InMemoryMailboxManager
{
    public Dictionary<string, Mailbox> Mailboxes { get; } = [];

    public Dictionary<DomainType, string> Domains { get; } = new()
    {
        { DomainType.Custom, "custom.com" },
        { DomainType.Public, "public.com" },
        { DomainType.Premium, "premium.com" }
    };


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

    public void AddMessage(Message message)
    {
        if (Mailboxes.TryGetValue(message.From, out var fromMailbox))
        {
            fromMailbox.Messages.Add(message.Id, message);
        }

        if (Mailboxes.TryGetValue(message.To, out var toMailbox))
        {
            toMailbox.Messages.Add(message.Id, message);
        }
    }

}

public class Mailbox(string email)
{
    public string Email { get; } = email;

    public Dictionary<string, Message> Messages { get; set; } = [];

    public Dictionary<string, Attachment> Attachments { get; set; } = [];
}