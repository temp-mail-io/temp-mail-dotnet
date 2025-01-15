using System.Text.Json;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class GetRateLimitStatusTests
{
    [SetUp]
    public void Setup()
    {
        SetUp.Handler.ReturnErrors(false);
    }
    
    [Test]
    public async Task TestGetRateLimitStatus()
    {
        var rateLimitResponse = await SetUp.Client.GetRateLimitStatus();
        Assert.That(rateLimitResponse, Is.Not.Null);
        Assert.That(rateLimitResponse.IsSuccess, Is.True, () => JsonSerializer.Serialize(rateLimitResponse));
        Assert.That(rateLimitResponse.Result, Is.Not.Null);
        Assert.That(rateLimitResponse.Result.Limit, Is.Positive);
        Assert.That(rateLimitResponse.Result.Remaining, Is.Positive);
        Assert.That(rateLimitResponse.Result.Reset, Is.Positive);
        Assert.That(rateLimitResponse.Result.Used, Is.Positive);
        Assert.That(rateLimitResponse.Result.ResetDateTime, Is.GreaterThan(DateTime.UtcNow));
    }
    
    [Test]
    public async Task TestGetRateLimitStatus_Error()
    {
        SetUp.Handler.ReturnErrors();
        
        var domainsResponse = await SetUp.Client.GetRateLimitStatus();
        
        Assert.That(domainsResponse, Is.Not.Null);
        Assert.That(domainsResponse.IsSuccess, Is.False);
        Assert.That(domainsResponse.ErrorResult, Is.Not.Null);
        Assert.That(domainsResponse.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}