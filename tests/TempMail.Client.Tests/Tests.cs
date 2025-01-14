using TempMail.Client.Models;
using TempMail.Client.Requests;
using TempMail.Client.Responses;

namespace TempMail.Client.Tests;

public class Tests
{
    private ITempMailClient Client => TestsSetUp.Client;
    private MockingHttpMessageHandler HttpMessageHandler => TestsSetUp.Handler;

    [SetUp]
    public void Setup()
    {
        HttpMessageHandler.ReturnErrors(false);
    }

    [Test]
    public async Task TestCreateEmail_ByEmail(
        [Values("test@test.com", "some@email.com")] string email)
    {
        var resp = await Client.CreateEmail(CreateEmailRequest.ByEmail(email), CancellationToken.None);
        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.IsSuccess, Is.True);
        Assert.That(resp.Result, Is.Not.Null);
        Assert.That(resp.Result.Email, Is.EqualTo(email));
        Assert.That(resp.Result.Ttl, Is.Positive);
    }

    [Test]
    public async Task TestCreateEmail_ByDomain([Values("mega-domain.com", "also-pretty-domain.io")] string domain)
    {
        var resp = await Client.CreateEmail(CreateEmailRequest.ByDomain(domain), CancellationToken.None);
        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.IsSuccess, Is.True);
        Assert.That(resp.Result, Is.Not.Null);
        Assert.That(resp.Result.Email, Is.EqualTo($"random@{domain}"));
        Assert.That(resp.Result.Ttl, Is.Positive);
    }

    [Test]
    public async Task TestCreateEmail_ByDomainType([Values(DomainType.Public, DomainType.Custom, DomainType.Premium)] DomainType domainType)
    {
        var resp = await Client.CreateEmail(CreateEmailRequest.ByDomainType(domainType), CancellationToken.None);
        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.IsSuccess, Is.True);
        Assert.That(resp.Result, Is.Not.Null);
        Assert.That(resp.Result.Email, Is.EqualTo($"random@{domainType.ToString().ToLowerInvariant()}.io"));
        Assert.That(resp.Result.Ttl, Is.Positive);
    }

    [Test]
    public async Task TestCreateEmail_Error()
    {
        HttpMessageHandler.ReturnErrors();
        var errResp = await Client.CreateEmail(CreateEmailRequest.ByEmail("test@test.com"), CancellationToken.None);
        Assert.That(errResp, Is.Not.Null);
        Assert.That(errResp.IsSuccess, Is.False);
        Assert.That(errResp.ErrorResult, Is.Not.Null);
        Assert.That(errResp.ErrorResult.Error.Type, Is.EqualTo(ErrorType.ApiError));
    }
}

[SetUpFixture]
public class TestsSetUp
{
    internal static ITempMailClient Client { get; private set; } = null!;
    internal static MockingHttpMessageHandler Handler { get; private set; } = null!;
    
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        Handler = new MockingHttpMessageHandler();
        Client = TempMailClient.Create(
            TempMailClientConfigurationBuilder.Create()
            .WithApiKey("api-key")
            .Build(),
            new HttpClient(Handler),
            true);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        Client.Dispose();
        Handler.Dispose();
    }
}