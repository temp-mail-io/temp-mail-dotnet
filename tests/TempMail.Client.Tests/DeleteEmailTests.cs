using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class DeleteEmailTests
{
    private Response<CreateEmailResponse> Email { get; set; }

    [SetUp]
    public async Task Setup()
    {
        SetUp.Handler.ReturnErrors(false);
        
        Email = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Custom));
        
        Assert.That(Email, Is.Not.Null);
        Assert.That(Email.IsSuccess, Is.True, () => Email.ErrorResult.Error.Detail);
        Assert.That(Email.Result, Is.Not.Null);
    }

    [Test]
    public async Task TestDeleteEmail()
    {
        var deleted = await SetUp.Client.DeleteEmail(DeleteEmailRequest.Create(Email.Result!.Email));
        
        Assert.That(deleted.IsSuccess, Is.True, () => deleted.ErrorResult.Error.Detail);
        Assert.That(deleted.Result, Is.Not.Null);
    }

    [Test]
    public async Task TestDeleteEmail_Error()
    {
        var deleted = await SetUp.Client.DeleteEmail(DeleteEmailRequest.Create(Email.Result!.Email));
        
        Assert.That(deleted.IsSuccess, Is.True, () => deleted.ErrorResult.Error.Detail);
        Assert.That(deleted.Result, Is.Not.Null);
        
        var messages = await SetUp.Client.GetAllMessages(GetAllMessagesRequest.Create(Email.Result!.Email));
        Assert.That(messages.IsSuccess, Is.False);
        Assert.That(messages.ErrorResult, Is.Not.Null);
        Assert.That(messages.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}