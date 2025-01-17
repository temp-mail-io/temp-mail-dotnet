using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class DeleteSpecificMessageTests
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
    public async Task TestDeleteSpecificMessage()
    {
        var messageResponse = await SetUp.Client.GetSpecificMessage(GetSpecificMessageRequest.Create(Message.Id));
        Assert.That(messageResponse, Is.Not.Null);
        Assert.That(messageResponse.IsSuccess, Is.True, () => messageResponse.ErrorResult!.Error.Detail);
        Assert.That(messageResponse.Result, Is.Not.Null);
        Assert.That(messageResponse.Result.Id, Is.EqualTo(Message.Id));

        var deletionResponse = await SetUp.Client.DeleteSpecificMessage(DeleteSpecificMessageRequest.Create(Message.Id));
        Assert.That(deletionResponse, Is.Not.Null);
        Assert.That(deletionResponse.IsSuccess, Is.True, () => deletionResponse.ErrorResult!.Error.Detail);
        Assert.That(deletionResponse.Result, Is.Not.Null);

        var errorMessageResponse = await SetUp.Client.GetSpecificMessage(GetSpecificMessageRequest.Create(Message.Id));
        Assert.That(errorMessageResponse, Is.Not.Null);
        Assert.That(errorMessageResponse.IsSuccess, Is.False);
        Assert.That(errorMessageResponse.ErrorResult, Is.Not.Null);
        Assert.That(errorMessageResponse.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }

    [Test]
    public async Task TestDeleteSpecificMessage_Error()
    {
        SetUp.Handler.ReturnErrors();

        var deletionResponse = await SetUp.Client.DeleteSpecificMessage(DeleteSpecificMessageRequest.Create(Message.Id));
        Assert.That(deletionResponse, Is.Not.Null);
        Assert.That(deletionResponse.IsSuccess, Is.False);
        Assert.That(deletionResponse.ErrorResult, Is.Not.Null);
        Assert.That(deletionResponse.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}