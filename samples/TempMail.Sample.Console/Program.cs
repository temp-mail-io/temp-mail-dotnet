// See https://aka.ms/new-console-template for more information

using TempMail.Client;
using TempMail.Client.Models;
using TempMail.Client.Requests;

ITempMailClient client = TempMailClient
    .Create(TempMailClientConfigurationBuilder
        .Create()
        .WithApiKey(/*paste your API-Key here*/ null)
        .Build());
        
var domainsResponse = await client.GetAvailableDomains();

if (domainsResponse is not { IsSuccess: true, Result.Domains: { } domains })
{
    return;
}

foreach (var domain in domains)
{
    Console.WriteLine("Type: {0}; Domain: {1}", domain.Type, domain.Name);
}

var emailResponse = await client.CreateEmail(CreateEmailRequest.ByDomainType(DomainType.Public));

if (emailResponse is not { IsSuccess: true, Result: { } email })
{
    return;
}

Console.WriteLine("Ttl: {0}; Email: {1}", email.Ttl, email.Email);

var deleteEmailResponse = await client.DeleteEmail(DeleteEmailRequest.Create(emailResponse.Result.Email));

Console.WriteLine("Deleted email: {0}", deleteEmailResponse.IsSuccess);

var rateLimitStatusResponse = await client.GetRateLimitStatus();

if (rateLimitStatusResponse is not { IsSuccess: true, Result: { } rateLimitStatus })
{
    return;
}

Console.WriteLine("Limit: {0}; Used: {1}; Remaining: {2}; Reset date-time: {3}",
    rateLimitStatus.Limit,
    rateLimitStatus.Used,
    rateLimitStatus.Remaining,
    rateLimitStatus.ResetDateTime);
    
    
client.Dispose();