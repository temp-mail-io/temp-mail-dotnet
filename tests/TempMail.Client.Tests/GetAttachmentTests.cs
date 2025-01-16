using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class GetAttachmentTests
{
    private Response<CreateEmailResponse> Email { get; set; }

    [SetUp]
    public async Task Setup()
    {
        SetUp.Handler.ReturnErrors(false);

        Email = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Custom));

        Assert.That(Email, Is.Not.Null);
        Assert.That(Email.IsSuccess, Is.True, () => Email.ErrorResult!.Error.Detail);
        Assert.That(Email.Result, Is.Not.Null);

        var message = new Message(
            Guid.NewGuid().ToString(),
            Email.Result.Email,
            "someone@somewhere.com",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            DateTime.UtcNow,
            []);

        SetUp.MailboxManager.AddMessage(message);
        var attachment = "This is a test attachment"u8.ToArray();
        SetUp.MailboxManager.AddAttachment(message.From, message.To, message.Id, attachment);
    }

    [Test]
    public async Task TestGetAttachment()
    {
        var messagesResponse = await SetUp.Client.GetAllMessages(GetAllMessagesRequest.Create(Email.Result!.Email));
        Assert.That(messagesResponse, Is.Not.Null);
        Assert.That(messagesResponse.IsSuccess, Is.True, () => messagesResponse.ErrorResult!.Error.Detail);
        Assert.That(messagesResponse.Result, Is.Not.Null);
        Assert.That(messagesResponse.Result.Messages.Length, Is.EqualTo(1));

        var message = messagesResponse.Result.Messages[0];

        Assert.That(message, Is.Not.Null);
        Assert.That(message.Attachments.Length, Is.EqualTo(1));

        var attachment = message.Attachments[0];
        var testAttachment = "This is a test attachment"u8.ToArray();

        Assert.That(attachment, Is.Not.Null);
        Assert.That(attachment.Size, Is.EqualTo(testAttachment.Length));

        var attachmentResponse = await SetUp.Client.GetAttachment(GetAttachmentRequest.Create(attachment.Id));

        Assert.That(attachmentResponse, Is.Not.Null);
        Assert.That(attachmentResponse.IsSuccess, Is.True, () => attachmentResponse.ErrorResult!.Error.Detail);
        Assert.That(attachmentResponse.Result, Is.Not.Null);
        Assert.That(attachmentResponse.Result.Length, Is.EqualTo(testAttachment.Length));
        Assert.That(testAttachment, Is.EquivalentTo(attachmentResponse.Result));
    }

    [Test]
    public async Task TestGetAttachment_Error()
    {
        var attachmentResponse = await SetUp.Client.GetAttachment(GetAttachmentRequest.Create("nonexistent-attachment-id"));

        Assert.That(attachmentResponse, Is.Not.Null);
        Assert.That(attachmentResponse.IsSuccess, Is.False);
        Assert.That(attachmentResponse.Result, Is.Null);
        Assert.That(attachmentResponse.ErrorResult, Is.Not.Null);
        Assert.That(attachmentResponse.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}