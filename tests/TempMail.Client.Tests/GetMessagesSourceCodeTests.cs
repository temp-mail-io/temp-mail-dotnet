using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class GetMessagesSourceCodeTests
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
            "body",
            "<div>body</div>",
            DateTime.UtcNow,
            []);

        SetUp.MailboxManager.AddMessage(Message);
    }

    [Test]
    public async Task TestGetMessagesSourceCode()
    {
        var sourceCodeResponse = await SetUp.Client.GetMessageSourceCode(GetMessageSourceCodeRequest.Create(Message.Id));

        Assert.That(sourceCodeResponse, Is.Not.Null);
        Assert.That(sourceCodeResponse.IsSuccess, Is.True, () => sourceCodeResponse.ErrorResult!.Error.Detail);
        Assert.That(sourceCodeResponse.Result, Is.Not.Null);
        Assert.That(sourceCodeResponse.Result, Is.EqualTo(Message.BodyHtml));
    }

    [Test]
    public async Task TestGetMessagesSourceCode_Error()
    {
        SetUp.Handler.ReturnErrors();

        var sourceCodeResponse = await SetUp.Client.GetMessageSourceCode(GetMessageSourceCodeRequest.Create(Message.Id));

        Assert.That(sourceCodeResponse, Is.Not.Null);
        Assert.That(sourceCodeResponse.IsSuccess, Is.False);
        Assert.That(sourceCodeResponse.ErrorResult, Is.Not.Null);
        Assert.That(sourceCodeResponse.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}