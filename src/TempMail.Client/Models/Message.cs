using System;

namespace TempMail.Client.Models;

/// <summary>
/// E-mail message
/// </summary>
/// <param name="id">E-mail message ID</param>
/// <param name="from">E-mail message sender e-mail</param>
/// <param name="to">E-mail message recipient e-mail</param>
/// <param name="cc">"Carbon copy", additional recipients e-mails</param>
/// <param name="subject">E-mail message subject</param>
/// <param name="bodyText">E-mail message body text representation</param>
/// <param name="bodyHtml">E-mail message body HTML representation</param>
/// <param name="createdAt">Timestamp at which the e-mail message was created</param>
/// <param name="attachments">List of <see cref="Attachment"/>s</param>
public class Message(
    string id,
    string from,
    string to,
    string[] cc,
    string subject,
    string bodyText,
    string bodyHtml,
    DateTime createdAt,
    Attachment[] attachments)
{
    /// <summary>
    /// E-mail message ID
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// E-mail message sender e-mail
    /// </summary>
    public string From { get; } = from;

    /// <summary>
    /// E-mail message recipient e-mail
    /// </summary>
    public string To { get; } = to;

    /// <summary>
    /// "Carbon copy", additional recipients e-mails
    /// </summary>
    public string[] Cc { get; } = cc;

    /// <summary>
    /// E-mail message subject
    /// </summary>
    public string Subject { get; } = subject;

    /// <summary>
    /// E-mail message body text representation
    /// </summary>
    public string BodyText { get; } = bodyText;

    /// <summary>
    /// E-mail message body HTML representation
    /// </summary>
    public string BodyHtml { get; } = bodyHtml;

    /// <summary>
    /// Timestamp at which the e-mail message was created
    /// </summary>
    public DateTime CreatedAt { get; } = createdAt;

    /// <summary>
    /// List of <see cref="Attachment"/>s
    /// </summary>
    public Attachment[] Attachments { get; } = attachments;
}