using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class GetAllMessagesTests
{
    [SetUp]
    public async Task Setup()
    {
        SetUp.Handler.ReturnErrors(false);
        
        PremiumEmail = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Premium));
        
        Assert.That(PremiumEmail, Is.Not.Null);
        Assert.That(PremiumEmail.IsSuccess, Is.True, () => PremiumEmail.ErrorResult.Error.Detail);
        Assert.That(PremiumEmail.Result, Is.Not.Null);
        
        PublicEmail = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Public));
        
        Assert.That(PublicEmail, Is.Not.Null);
        Assert.That(PublicEmail.IsSuccess, Is.True, () => PublicEmail.ErrorResult.Error.Detail);
        Assert.That(PublicEmail.Result, Is.Not.Null);
    }

    private Response<CreateEmailResponse> PremiumEmail { get; set; }
    private Response<CreateEmailResponse> PublicEmail { get; set; }

    [Test]
    public async Task TestGetAllMessages()
    {
        SetUp.MailboxManager.AddMessage(new Message(
            Guid.NewGuid().ToString("N"),
            PremiumEmail.Result!.Email,
            PublicEmail.Result!.Email,
            string.Empty,
            "Very important message",
            "Header",
            "<h1>Header</h1>",
            DateTime.Now, 
            []));

        var messages = await SetUp.Client.GetAllMessages(GetAllMessagesRequest.Create(PublicEmail.Result.Email));
        
        Assert.That(messages.IsSuccess, Is.True, () => messages.ErrorResult.Error.Detail);
        Assert.That(messages.Result, Is.Not.Null);
        Assert.That(messages.Result.Messages.Length, Is.EqualTo(1));
    }

    [Test]
    public async Task TestGetAllMessages_Error()
    {
        SetUp.Handler.ReturnErrors();
        
        SetUp.MailboxManager.AddMessage(new Message(
            Guid.NewGuid().ToString("N"),
            PremiumEmail.Result!.Email,
            PublicEmail.Result!.Email,
            string.Empty,
            "Very important message",
            "Header",
            "<h1>Header</h1>",
            DateTime.Now, 
            []));

        var messages = await SetUp.Client.GetAllMessages(GetAllMessagesRequest.Create(PublicEmail.Result.Email));
        
        Assert.That(messages.IsSuccess, Is.False);
        Assert.That(messages.ErrorResult, Is.Not.Null);
        Assert.That(messages.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}