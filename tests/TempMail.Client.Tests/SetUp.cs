using TempMail.Client.Tests.Util;

namespace TempMail.Client.Tests;

[SetUpFixture]
public class SetUp
{
    internal static ITempMailClient Client { get; private set; } = null!;
    internal static MockingHttpMessageHandler Handler { get; private set; } = null!;
    internal static InMemoryMailboxManager MailboxManager { get; private set; } = null!;
    
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        MailboxManager = new InMemoryMailboxManager();
        Handler = new MockingHttpMessageHandler(MailboxManager);
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