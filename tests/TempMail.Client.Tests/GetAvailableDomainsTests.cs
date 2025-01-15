using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class GetAvailableDomainsTests
{
    [Test]
    public async Task TestGetAvailableDomains()
    {
        var domainsResponse = await SetUp.Client.GetAvailableDomains();
        
        Assert.That(domainsResponse, Is.Not.Null);
        Assert.That(domainsResponse.IsSuccess, Is.True, () => domainsResponse.ErrorResult.Error.Detail);
        Assert.That(domainsResponse.Result, Is.Not.Null);
    }
    
    [Test]
    public async Task TestGetAvailableDomains_Error()
    {
        SetUp.Handler.ReturnErrors();
        
        var domainsResponse = await SetUp.Client.GetAvailableDomains();
        
        Assert.That(domainsResponse, Is.Not.Null);
        Assert.That(domainsResponse.IsSuccess, Is.False);
        Assert.That(domainsResponse.ErrorResult, Is.Not.Null);
        Assert.That(domainsResponse.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}