using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class CreateEmailRequestTests
{
    [SetUp]
    public void Setup()
    {
        SetUp.Handler.ReturnErrors(false);
    }

    [Test]
    public async Task TestCreateEmail_ByEmail(
        [Values("test@test.com", "some@email.com")] string email)
    {
        var resp = await SetUp.Client.CreateEmail(CreateEmailRequest.ByEmail(email), CancellationToken.None);
        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.IsSuccess, Is.True, () => resp.ErrorResult.Error.Detail);
        Assert.That(resp.Result, Is.Not.Null);
        Assert.That(resp.Result.Email, Is.EqualTo(email));
        Assert.That(resp.Result.Ttl, Is.Positive);
    }

    [Test]
    public async Task TestCreateEmail_ByDomain([Values("mega-domain.com", "also-pretty-domain.io")] string domain)
    {
        var resp = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomain(domain), CancellationToken.None);
        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.IsSuccess, Is.True, () => resp.ErrorResult.Error.Detail);
        Assert.That(resp.Result, Is.Not.Null);
        Assert.That(resp.Result.Email, Does.EndWith(domain));
        Assert.That(resp.Result.Ttl, Is.Positive);
    }

    [Test]
    public async Task TestCreateEmail_ByDomainType([Values(DomainType.Public, DomainType.Custom, DomainType.Premium)] DomainType domainType)
    {
        var resp = await SetUp.Client.CreateEmail(CreateEmailRequest.ByDomainType(domainType), CancellationToken.None);
        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.IsSuccess, Is.True, () => resp.ErrorResult.Error.Detail);
        Assert.That(resp.Result, Is.Not.Null);
        Assert.That(resp.Result.Email, Does.Contain(domainType.ToString().ToLowerInvariant()));
        Assert.That(resp.Result.Ttl, Is.Positive);
    }

    [Test]
    public async Task TestCreateEmail_Error()
    {
        SetUp.Handler.ReturnErrors();
        var errResp = await SetUp.Client.CreateEmail(CreateEmailRequest.ByEmail("test@test.com"), CancellationToken.None);
        Assert.That(errResp, Is.Not.Null);
        Assert.That(errResp.IsSuccess, Is.False);
        Assert.That(errResp.ErrorResult, Is.Not.Null);
        Assert.That(errResp.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}