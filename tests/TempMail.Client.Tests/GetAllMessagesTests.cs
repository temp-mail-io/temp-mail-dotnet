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
        
        Assert.IsNotNull(PremiumEmail);
        Assert.IsTrue(PremiumEmail.IsSuccess);
        Assert.IsNotNull(PremiumEmail.Result);
        
        PublicEmail = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Public));
        
        Assert.IsNotNull(PublicEmail);
        Assert.IsTrue(PublicEmail.IsSuccess);
        Assert.IsNotNull(PublicEmail.Result);
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
        
        Assert.IsTrue(messages.IsSuccess);
        Assert.IsNotNull(messages.Result);
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
        
        Assert.IsFalse(messages.IsSuccess);
        Assert.IsNotNull(messages.ErrorResult);
        Assert.That(messages.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}