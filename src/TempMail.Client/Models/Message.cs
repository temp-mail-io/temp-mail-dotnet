using System;

namespace TempMail.Client.Models;

public class Message(
    string id,
    string from,
    string to,
    string cc,
    string subject,
    string bodyText,
    string bodyHtml,
    DateTime createdAt,
    Attachment[] attachments)
{
    public string Id { get; } = id;
    public string From { get; } = from;
    public string To { get; } = to;
    public string Cc { get; } = cc;
    public string Subject { get; } = subject;
    public string BodyText { get; } = bodyText;
    public string BodyHtml { get; } = bodyHtml;
    public DateTime CreatedAt { get; } = createdAt;
    public Attachment[] Attachments { get; } = attachments;
}