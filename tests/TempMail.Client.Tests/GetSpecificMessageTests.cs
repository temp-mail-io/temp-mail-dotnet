using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class GetSpecificMessageTests
{
    private Response<CreateEmailResponse> Email { get; set; }
    private Message Message { get; set; }

    [SetUp]
    public async Task Setup()
    {
        SetUp.Handler.ReturnErrors(false);

        Email = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Custom));

        Assert.That(Email, Is.Not.Null);
        Assert.That(Email.IsSuccess, Is.True, () => Email.ErrorResult!.Error.Detail);
        Assert.That(Email.Result, Is.Not.Null);

        Message = new Message(
            Guid.NewGuid().ToString(),
            Email.Result.Email,
            "someone@somewhere.com",
            [],
            string.Empty,
            string.Empty,
            string.Empty,
            DateTime.UtcNow,
            []);

        SetUp.MailboxManager.AddMessage(Message);
    }

    [Test]
    public async Task TestGetSpecificMessage()
    {
        var specificMessage = await SetUp.Client.GetSpecificMessage(GetSpecificMessageRequest.Create(Message.Id));

        Assert.That(specificMessage, Is.Not.Null);
        Assert.That(specificMessage.IsSuccess, Is.True, () => specificMessage.ErrorResult!.Error.Detail);
        Assert.That(specificMessage.Result, Is.Not.Null);
        Assert.That(specificMessage.Result.Id, Is.EqualTo(Message.Id));
        Assert.That(specificMessage.Result.From, Is.EqualTo(Email.Result!.Email));

        var allMessages = await SetUp.Client.GetAllMessages(GetAllMessagesRequest.Create(Email.Result.Email));

        Assert.That(allMessages, Is.Not.Null);
        Assert.That(allMessages.IsSuccess, Is.True, () => allMessages.ErrorResult!.Error.Detail);
        Assert.That(allMessages.Result, Is.Not.Null);
        Assert.That(allMessages.Result.Messages.Count, Is.EqualTo(1));

        Assert.That(allMessages.Result.Messages[0].Id, Is.EqualTo(specificMessage.Result.Id));
    }

    [Test]
    public async Task TestGetSpecificMessage_Error()
    {
        SetUp.Handler.ReturnErrors();

        var specificMessage = await SetUp.Client.GetSpecificMessage(GetSpecificMessageRequest.Create(Message.Id));

        Assert.That(specificMessage, Is.Not.Null);
        Assert.That(specificMessage.IsSuccess, Is.False);
        Assert.That(specificMessage.Result, Is.Null);
        Assert.That(specificMessage.ErrorResult, Is.Not.Null);
        Assert.That(specificMessage.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}